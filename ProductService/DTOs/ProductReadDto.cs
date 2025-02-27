using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs;

public class ProductReadDto
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public double Price { get; set; }
}
