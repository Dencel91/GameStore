using UserService.Dtos;

namespace UserService.Models
{
    public class User
    {
        public Guid Id { get; set; }
        
        public required string Name { get; set; }

        public string? Email { get; set; }

        //public virtual ICollection<ProductDto> Products { get; set; } = new List<ProductDto>();

        //public virtual ICollection<User> Friends { get; set; } = new List<User>();
    }
}
