using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Models;

public class ProductImage
{
    [Key, Column(Order = 0)]
    [Required]
    public int ProductId { get; set; }

    [Key, Column(Order = 1)]
    [Required]
    public required string ImageUrl { get; set; }

    [Required]
    public int Order { get; set; }

    public virtual Product Product { get; set; }
}
