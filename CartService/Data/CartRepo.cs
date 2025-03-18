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

    public Task<Cart> GetCartById(int id)
    {
        return _context.Carts.FirstOrDefaultAsync(cart => cart.Id == id);
    }

    public Task<bool> CartExists(int id)
    {
        return _context.Carts.AnyAsync(c => c.Id == id);
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}
