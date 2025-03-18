using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CartService.Models
{
    public class CartProduct
    {
        [Key, Column(Order = 0)]
        public int CartId { get; set; }

        [Key, Column(Order = 1)]
        public int ProductId { get; set; }
    }
}
