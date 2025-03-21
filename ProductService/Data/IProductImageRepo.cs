using ProductService.Models;

namespace ProductService.Data
{
    public interface IProductImageRepo
    {
        Task<ProductImage> CreateProductImage(ProductImage productImage);

        Task<IEnumerable<ProductImage>> GetImagesByProductId(int productId);

        Task<bool> SaveChanges();
    }
}
