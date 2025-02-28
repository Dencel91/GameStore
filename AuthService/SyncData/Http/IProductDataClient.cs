using UserService.Models;

namespace UserService.SyncData.Http;

public interface IProductDataClient
{
    Task<IEnumerable<Product>> GetProductsByUserId(int id);
}
 