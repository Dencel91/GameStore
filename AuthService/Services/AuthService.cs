using AuthService.Data;
using AuthService.DTOs;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Services;

public class AuthService(IUserRepo userRepo, IConfiguration configuration) : IAuthService
{
    public async Task<UserDto> Register(RegisterRequest request)
    {
        await VerifyRegisterRequest(request);

        var user = new User() { Name = request.UserName!, Role = "User", PasswordHash = string.Empty };
        var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password!);
        user.PasswordHash = hashedPassword;

        await userRepo.AddUser(user);
        await userRepo.SaveChanges();

        var userDto = new UserDto()
        {
            Id = user.Id
        };

        return userDto;
    }

    private async Task VerifyRegisterRequest(RegisterRequest request)
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

        var userExists = await userRepo.UserExists(request.UserName);
        if (userExists)
        {
            throw new ArgumentException("User with this name already exists");
        }
    }

    public async Task<TokenResponse> Login(LoginRequest request)
    {
        ValidateLoginRequest(request);

        var user = await userRepo.GetUserByName(request.UserName!) ?? throw new ArgumentException("Invalid user or password");

        if (new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, request.Password!) == PasswordVerificationResult.Failed)
        {
            throw new ArgumentException("Invalid user or password");
        }

        return await CreateTokenResponse(user);
    }

    private async Task<TokenResponse> CreateTokenResponse(User user)
    {
        return new TokenResponse()
        {
            AccessToken = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshToken(user)
        };
    }

    private void ValidateLoginRequest(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
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

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Authorization:Token"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: configuration["Authorization:Issuer"],
            audience: configuration["Authorization:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
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
