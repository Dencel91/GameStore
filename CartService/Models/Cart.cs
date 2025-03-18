using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CartService.Models;

public class Cart
{
    [Key]
    [Required]
    public int Id { get; set; }

    public int UserId { get; set; }

    [NotMapped]
    public IEnumerable<Product> Products { get; set; } = new List<Product>();

    public double TotalPrice { get; set; }
}
