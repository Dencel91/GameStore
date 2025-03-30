namespace UserService.Models
{
    public class User
    {
        public Guid Id { get; set; }
        
        public required string Name { get; set; }

        public string? Email { get; set; }

        public virtual ICollection<UserProduct> Products { get; set; } = new List<UserProduct>();
    }
}
