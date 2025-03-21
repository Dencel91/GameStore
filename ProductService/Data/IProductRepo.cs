using ProductService.Models;

namespace ProductService.Data;

public interface IProductRepo
{
    Task<bool> SaveChanges();

    Task<IEnumerable<Product>> GetAllProducts();

    Task<Product?> GetProduct(int id);

    Task<Product> CreateProduct(Product product);

    Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<int> ids);

    Task<Product?> GetProductDetails(int id);
}
