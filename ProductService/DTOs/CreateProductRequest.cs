﻿using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs;

public class CreateProductRequest
{
    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public double Price { get; set; }

    [Required]
    public IFormFile? Thumbnail { get; set; }

    [Required]
    public IEnumerable<IFormFile> Images { get; set; } = [];
}
