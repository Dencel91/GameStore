using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs;

public class ProductCreateDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Description { get; set; }

    [Required]
    public double Price { get; set; }
}
