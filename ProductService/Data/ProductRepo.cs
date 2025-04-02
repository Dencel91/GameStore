using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data;

public class ProductRepo : IProductRepo
{
    private readonly AppDbContext _context;

    public ProductRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SaveChanges()
    {
        var updated = await _context.SaveChangesAsync() >= 0;
        return updated;
    }

    public async Task<Product> CreateProduct(Product product)
    {
        await _context.Products.AddAsync(product);
        return product;
    }

    public async Task<IEnumerable<Product>> GetPagedProducts(int pageCursor, int pageSize)
    {
        var products = await _context.Products.Where(p => p.Id > pageCursor).Take(pageSize).ToListAsync();
        return products;
    }

    public Task<Product?> GetProduct(int id)
    {
        return _context.Products.FirstOrDefaultAsync(product => product.Id == id);
    }

    public async Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<int> ids)
    {
        var products = await _context.Products.Where(product => ids.Contains(product.Id)).ToListAsync();
        return products;
    }

    public Task<Product?> GetProductDetails(int id)
    {
        return _context.Products.Include(p => p.Images.OrderBy(i => i.Order)).FirstOrDefaultAsync(p => p.Id == id);
    }
}
