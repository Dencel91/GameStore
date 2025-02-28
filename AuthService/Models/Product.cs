﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserService.Models;

public class Product
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("price")]
    public double Price { get; set; }
}
