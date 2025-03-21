using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data;

public class ProductReviewRepo : IProductReviewRepo
{
    private readonly AppDbContext _context;

    public ProductReviewRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductReview> CreateProductReview(ProductReview review)
    {
        await _context.ProductReviews.AddAsync(review);
        return review;
    }

    public async Task<ICollection<ProductReview>> GetReviewsByProductId(int productId)
    {
        var reviews = await _context.ProductReviews.Where(i => i.ProductId == productId).ToListAsync();
        return reviews;
    }

    public async Task<bool> SaveChanges()
    {
        var updated = await _context.SaveChangesAsync() >= 0;
        return updated;
    }
}
