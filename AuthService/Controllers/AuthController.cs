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
            try
            {
                var userId = await authService.Register(request);
                return Ok(userId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest requet)
        {
            try
            {
                var result = await authService.Login(requet);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("google-login")]
        public async Task<ActionResult<TokenResponse>> GoogleLogin([FromBody] string credential)
        {
            try
            {
                var result = await authService.GoogleLogin(credential);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
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
