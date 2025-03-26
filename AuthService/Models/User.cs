namespace AuthService.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string PasswordHash { get; set; }

        public required string Role { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
