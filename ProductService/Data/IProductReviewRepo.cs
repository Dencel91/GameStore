using ProductService.Models;

namespace ProductService.Data
{
    public interface IProductReviewRepo
    {
        Task<ProductReview> CreateProductReview(ProductReview review);

        Task<ICollection<ProductReview>> GetReviewsByProductId(int productId);

        Task<bool> SaveChanges();
    }
}
