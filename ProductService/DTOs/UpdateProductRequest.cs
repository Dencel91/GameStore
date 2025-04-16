using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs;

public class UpdateProductRequest
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public double Price { get; set; }

    //[Required]
    public IFormFile? UpdatedThumbnail { get; set; }

    //[Required]
    public IEnumerable<IFormFile> NewImages { get; set; } = [];

    public IList<string> RemovedImages { get; set; } = [];
}
