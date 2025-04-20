using AuthService.Data;
using AuthService.DataServices;
using AuthService.DTOs;
using AuthService.Events;
using AuthService.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Services;

public class AuthService(
    IUserRepo userRepo,
    IConfiguration configuration,
    IMessageBusClient messageBusClient,
    IHostEnvironment environment) : IAuthService
{
    public async Task<Guid> Register(RegisterRequest request)
    {
        await ValidateRegisterRequest(request);

        var user = new User() { Name = request.UserName!, Role = "User", PasswordHash = string.Empty, Email = request.Email! };
        var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password!);
        user.PasswordHash = hashedPassword;

        await userRepo.AddUser(user);
        await userRepo.SaveChanges();

        PublishUserRegistered(user);

        return user.Id;
    }

    private async Task ValidateRegisterRequest(RegisterRequest request)
    {
        ValidateRequest(request);

        var userExists = await userRepo.UserNameExists(request.UserName!);
        if (userExists)
        {
            throw new ArgumentException("User with this name already exists");
        }

        var emailExists = await userRepo.EmailExists(request.Email!);
        if (emailExists)
        {
            throw new ArgumentException("User with this email already exists");
        }
    }

    private void ValidateRequest<T>(T request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var validationResult = new List<ValidationResult>();
        var context = new ValidationContext(request);
        Validator.TryValidateObject(request, context, validationResult, validateAllProperties: true);

        if (validationResult.Any())
        {
            var builder = new StringBuilder();
            foreach (var validationError in validationResult)
            {
                builder.Append($"{validationError.ErrorMessage}\n");
            }

            throw new ArgumentException(builder.ToString());
        }
    }

    private void PublishUserRegistered(User user)
    {
        messageBusClient.PublishUserRegistered(new UserRegisteredEvent()
        {
            UserId = user.Id,
            UserName = user.Name,
            Email = user.Email

        });
    }

    public async Task<TokenResponse> Login(LoginRequest request)
    {
        ValidateRequest(request);

        var user = await userRepo.GetUserByName(request.UserName!) ?? throw new ArgumentException("Invalid user or password");

        if (new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, request.Password!) == PasswordVerificationResult.Failed)
        {
            throw new ArgumentException("Invalid user or password");
        }

        return await CreateTokenResponse(user);
    }

    public async Task<TokenResponse> GoogleLogin(string credential)
    {
        if (string.IsNullOrEmpty(credential))
        {
            throw new ArgumentException("Invalid Google credential");
        }

        var googleClientId = Environment.GetEnvironmentVariable("GoogleAuthClientId");
        if (string.IsNullOrEmpty(googleClientId))
        {
            throw new InvalidOperationException("Google client ID not found");
        }

        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new[] { googleClientId }
        };

        var payload = GoogleJsonWebSignature.ValidateAsync(credential, settings).Result;

        var user = await userRepo.GetUserByName(payload.Name);

        if (user is null)
        {
            user = await RegisterGoogle(payload);
        }

        return await CreateTokenResponse(user);
    }

    private async Task<User> RegisterGoogle(GoogleJsonWebSignature.Payload payload)
    {
        var user = new User()
        {
            Name = payload.Name,
            Email = payload.Email,
            Role = "User",
            PasswordHash = string.Empty,
            RegisterType = RegisterType.Google,
        };
        await userRepo.AddUser(user);
        await userRepo.SaveChanges();

        PublishUserRegistered(user);

        return user;
    }

    private async Task<TokenResponse> CreateTokenResponse(User user)
    {
        return new TokenResponse()
        {
            AccessToken = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshToken(user)
        };
    }

    private async Task<string> GenerateAndSaveRefreshToken(User user)
    {
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await userRepo.SaveChanges();
        return user.RefreshToken;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var AuthenticationToken = GetAuthenticationToken();

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(AuthenticationToken));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: configuration["Authentication:Issuer"],
            audience: configuration["Authentication:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        return token;
    }

    private string GetAuthenticationToken()
    {
        if (environment.IsDevelopment())
        {
            return configuration["Authentication:Token"]
                ?? throw new InvalidOperationException("Authentication token not found");
        }
        
        return Environment.GetEnvironmentVariable("AuthenticationToken")
            ?? throw new InvalidOperationException("Authentication token not found");
    }

    public async Task<TokenResponse> RefreshTokens(RefreshTokenRequest request)
    {
        var user = await ValidateRefreshTokenRequest(request);

        return await CreateTokenResponse(user);
    }

    private async Task<User> ValidateRefreshTokenRequest(RefreshTokenRequest request)
    {
        var user = await userRepo.GetUserById(request.UserId);

        if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new ArgumentException("Invalid refresh token");
        }

        return user;
    }
}
