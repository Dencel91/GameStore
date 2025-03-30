using Microsoft.EntityFrameworkCore;
using CartService.Models;

namespace CartService.Data;

public class CartRepo : ICartRepo
{
    private readonly AppDbContext _context;

    public CartRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cart> CreateCart()
    {
        var cart = new Cart();
        await _context.Carts.AddAsync(cart);
        return cart;
    }

    public void DeleteCart(Cart cart)
    {
        _context.Carts.Remove(cart);
    }

    public Task<Cart?> GetCartById(int id)
    {
        return _context.Carts.Include(c => c.Products).FirstOrDefaultAsync(cart => cart.Id == id);
    }

    public Task<bool> CartExists(int id)
    {
        return _context.Carts.AnyAsync(c => c.Id == id);
    }

    public Task<Cart?> GetCartByUserId(Guid userId)
    {
        return _context.Carts.Include(c => c.Products).FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<bool> SaveChanges()
    {
        var updated = await _context.SaveChangesAsync() >= 0;
        return updated;
    }
}
