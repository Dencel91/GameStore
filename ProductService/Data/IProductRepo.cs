using ProductService.Models;

namespace ProductService.Data;

public interface IProductRepo
{
    bool SaveChanges();

    IEnumerable<Product> GetAllProducts();

    Task<Product> GetProductById(int id);

    void CreateProduct(Product product);
}
