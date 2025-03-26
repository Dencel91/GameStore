using AuthService.DTOs;

namespace AuthService.Services;

public interface IAuthService
{
    Task<UserDto> Register(RegisterRequest request);

    Task<TokenResponse> Login(LoginRequest request);   

    Task<TokenResponse> RefreshTokens(RefreshTokenRequest request);
}
