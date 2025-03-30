using CartService.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CartService.Models;

public class Cart
{
    [Key]
    [Required]
    public int Id { get; set; }

    //[Key]
    //[Required]
    //public Guid Guid { get; set; }

    public Guid UserId { get; set; }

    [NotMapped]
    public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();

    public double TotalPrice { get; set; }
}
