using ProductService.Models;

namespace ProductService.Data;

public interface IProductToUserRepo
{
    bool SaveChanges();

    Task<IEnumerable<Product>> GetProductsByUserId(int userId);

    void AddProductToUser(int productId, int userId);
}
