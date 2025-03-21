using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Models
{
    public class ProductReview
    {
        [Key]
        [Column(Order = 0)]
        [Required]
        public int UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required]
        public int ProductId { get; set; }

        public DateTimeOffset Date { get; set; }

        [Required]
        public bool IsRecommended { get; set; }

        [Required]
        public required string Review { get; set; }
    }
}
