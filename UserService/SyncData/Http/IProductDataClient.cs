using UserService.Dtos;

namespace UserService.SyncData.Http;

public interface IProductDataClient
{
    Task<IEnumerable<ProductDto>> GetProductsByUserId(int id);
}
 