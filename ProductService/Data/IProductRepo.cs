using ProductService.Models;

namespace ProductService.Data;

public interface IProductRepo
{
    Task<bool> SaveChanges();

    Task<IEnumerable<Product>> GetPagedProducts(int pageCursor, int pageSize);

    Task<Product?> GetProduct(int id);

    Task<Product> CreateProduct(Product product);

    Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<int> ids);

    Task<Product?> GetProductDetails(int id);

    Task<IEnumerable<Product>> SearchProduct(string searchText);
}
