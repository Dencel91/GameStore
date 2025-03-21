using CartService.DTOs;
using System.ComponentModel.DataAnnotations;

namespace CartService.Events;

public class PurchaseCompletedEvent
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public IEnumerable<ProductDto> Products { get; set; } = [];

    [Required]
    public string EventTypeName { get; set; } = "Purchase completed";
}
