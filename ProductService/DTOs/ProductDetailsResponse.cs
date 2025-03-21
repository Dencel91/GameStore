using ProductService.Models;

namespace ProductService.DTOs;

public class ProductDetailsResponse
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public double Price { get; set; }

    public required string ThumbnailUrl { get; set; }

    public IEnumerable<string> Images { get; set; } = [];

    public IEnumerable<ProductReview> Reviews { get; set; } = [];
}
