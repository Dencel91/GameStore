using CartService.DTOs;
using System.ComponentModel.DataAnnotations;

namespace CartService.Events;

public class PurchaseCompletedEvent
{
    public Guid UserId { get; set; }

    public IEnumerable<int> ProductIds { get; set; } = [];

    [Required]
    public string EventTypeName { get; set; } = "Purchase completed";
}
