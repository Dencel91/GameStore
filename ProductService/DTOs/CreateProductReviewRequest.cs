using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs;

public class CreateProductReviewRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public bool IsRecommended { get; set; }

    [Required]
    public required string Review { get; set; }
}
