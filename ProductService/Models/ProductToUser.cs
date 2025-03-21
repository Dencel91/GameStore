using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Models;

public class ProductToUser
{
    [Key, Column(Order = 0)]
    public int ProductId { get; set; }

    [Key, Column(Order = 1)]
    public int UserId { get; set; }
}
