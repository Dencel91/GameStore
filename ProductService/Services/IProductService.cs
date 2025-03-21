using ProductService.DTOs;
using ProductService.Models;

namespace ProductService.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProducts();

    Task<IEnumerable<Product>> GetProductsByUserId(int userId);

    Task<Product?> GetProduct(int id);

    Task<Product> CreateProduct(CreateProductRequest request);

    Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<int> ids);

    Task<Product?> GetProductDetails(int id);

    Task<ProductReview> CreateProductReview(CreateProductReviewRequest request);
}
