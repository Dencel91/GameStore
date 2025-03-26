namespace AuthService.Models
{
    public class User
    {
        public int Id { get; set; }

        public required string UID { get; set; }

        public required string Name { get; set; }

        public required string Password { get; set; }

        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        public IEnumerable<User> Friends { get; set; } = new List<User>();
    }
}
