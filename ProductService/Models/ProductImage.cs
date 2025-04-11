using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProductService.Models.enums;
using System.ComponentModel.DataAnnotations;

namespace ProductService.Models;

public class ProductImage
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string FullPath { get; set; }

    [Required]
    public ImageType Type { get; set; }

    [Required]
    public required string Url { get; set; }

    public Product Product { get; set; }
}
