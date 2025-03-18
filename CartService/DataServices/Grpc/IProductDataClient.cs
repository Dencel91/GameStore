using CartService.Models;

namespace CartService.DataServices.Grpc;

public interface IProductDataClient
{
    IEnumerable<Product> GetProductsByUserId(int userId);

    Product GetProductById(int id);
}
