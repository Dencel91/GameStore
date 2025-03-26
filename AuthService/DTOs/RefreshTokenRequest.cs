using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class RefreshTokenRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string? RefreshToken { get; set; }
    }
}
