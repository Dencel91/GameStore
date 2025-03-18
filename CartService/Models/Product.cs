using System.ComponentModel.DataAnnotations;

namespace CartService.Models;

public class Product
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Description { get; set; }

    [Required]
    public double Price { get; set; }
}
