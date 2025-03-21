using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data;

public class ProductImageRepo : IProductImageRepo
{
    private readonly AppDbContext _context;

    public ProductImageRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductImage> CreateProductImage(ProductImage productImage)
    {
        await _context.ProductImages.AddAsync(productImage);
        return productImage;
    }

    public async Task<IEnumerable<ProductImage>> GetImagesByProductId(int productId)
    {
        var images = await _context.ProductImages.Where(i => i.ProductId == productId).ToListAsync();
        return images;
    }

    public async Task<bool> SaveChanges()
    {
        var updated = await _context.SaveChangesAsync() >= 0;
        return updated;
    }
}
