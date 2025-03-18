using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data;

public class ProductToUserRepo : IProductToUserRepo
{
    private readonly AppDbContext _context;

    public ProductToUserRepo(AppDbContext context)
    {
        _context = context;
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }

    void IProductToUserRepo.AddProductToUser(int productId, int userId)
    {
        _context.Add(new ProductToUser { ProductId = productId, UserId = userId });
    }

    public async Task<IEnumerable<Product>> GetProductsByUserId(int userId)
    {
        var productIds = _context.ProductToUsers.Where(p => p.UserId == userId).Select(p => p.ProductId).ToList();
        var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
        return products;
    }
}
