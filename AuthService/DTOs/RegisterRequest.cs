using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = "User name is required.")]
    [MinLength(3)]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Verify password is required.")]
    [Compare("Password")]
    public string? VerifyPassword { get; set; }
}
