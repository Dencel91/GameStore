using UserService.Dtos;

namespace UserService.DataServices.Grpc;

public interface IProductDataClient
{
    ProductDto GetProductById(int id);

    IEnumerable<ProductDto> GetProductsByIds(IEnumerable<int> ids);
}
