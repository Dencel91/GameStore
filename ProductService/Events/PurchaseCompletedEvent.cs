using ProductService.Models;
using System.ComponentModel.DataAnnotations;

namespace ProductService.Events;

public class PurchaseCompletedEvent
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public IEnumerable<Product> Products { get; set; } = [];

    [Required]
    public string EventTypeName { get; set; } = "Purchase completed";
}
