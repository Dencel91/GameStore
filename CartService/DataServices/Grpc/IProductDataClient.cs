using CartService.DTOs;

namespace CartService.DataServices.Grpc;

public interface IProductDataClient
{
    IEnumerable<ProductDto> GetProductsByUserId(int userId);

    ProductDto GetProductById(int id);

    IEnumerable<ProductDto> GetProductsByIds(IEnumerable<int> ids);
}
