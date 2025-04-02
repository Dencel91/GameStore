namespace ProductService.DTOs
{
    public class GetProductsResponse
    {
        public IEnumerable<ProductDto> Products { get; set; } = [];

        public int NextPageCursor { get; set; }
    }
}
