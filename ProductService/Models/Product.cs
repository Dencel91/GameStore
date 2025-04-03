using AutoMapper.Configuration.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ProductService.Models;

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

    [Required]
    public required string ThumbnailUrl { get; set; }

    public virtual ICollection<ProductImage> Images { get; set; } = [];

    public virtual ICollection<ProductReview>? Reviews { get; set; } = [];
}
