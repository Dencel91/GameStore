using AuthService.Models;

namespace AuthService.SyncData.Http;

public interface IProductDataClient
{
    Task<IEnumerable<Product>> GetProductsByUserId(int id);
}
 