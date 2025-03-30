using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class UserProduct
    {
        [Key, Column(Order = 0)]
        public Guid UserId { get; set; }

        [Key, Column(Order = 1)]
        public int ProductId { get; set; }

        public virtual User User { get; set; }
    }
}
