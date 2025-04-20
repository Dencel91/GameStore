using AuthService.DTOs;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : Controller
    {
        [HttpPost("register")]
        public async Task<ActionResult<Guid>> Register([FromBody] RegisterRequest request)
        {
            var userId = await authService.Register(request);
            return Ok(userId);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest requet)
        {
            var result = await authService.Login(requet);
            return Ok(result);
        }

        [HttpPost("google-login")]
        public async Task<ActionResult<TokenResponse>> GoogleLogin([FromBody] string credential)
        {
            var result = await authService.GoogleLogin(credential);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest requet)
        {
            try
            {
                var result = await authService.RefreshTokens(requet);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
