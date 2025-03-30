namespace CartService.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public IEnumerable<ProductDto> Products { get; set; } = [];

        public double TotalPrice { get; set; }
    }
}
