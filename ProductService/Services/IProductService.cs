using ProductService.Models;

namespace ProductService.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsByUserId(int userId);

        Task<Product> GetProductById(int id);
    }
}
