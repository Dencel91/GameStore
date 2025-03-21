using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs;

public class CreateProductRequest
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Description { get; set; }

    [Required]
    public double Price { get; set; }

    [Required]
    public required string ThumbnailUrl { get; set; }

    [Required]
    public required IEnumerable<string> ImageUrls { get; set; }
}
