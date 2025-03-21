using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs;

public class ProductResponse
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public double Price { get; set; }

    public required string ThumbnailUrl { get; set; }
}
