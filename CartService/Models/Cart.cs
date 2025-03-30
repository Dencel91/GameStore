using System.ComponentModel.DataAnnotations;

namespace CartService.Models;

public class Cart
{
    [Key]
    [Required]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public virtual ICollection<CartProduct> Products { get; set; } = [];
}
