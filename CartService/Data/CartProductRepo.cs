using Microsoft.EntityFrameworkCore;
using CartService.Models;

namespace CartService.Data;

public class CartProductRepo : ICartProductRepo
{
    private readonly AppDbContext _context;

    public CartProductRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CartProduct>> GetProductsByCartId(int cartId)
    {
        return await _context.CartProducts.Where(cp => cp.CartId == cartId).ToListAsync();
    }

    public void AddProduct(CartProduct cartProduct)
    {
        _context.CartProducts.Add(cartProduct);
    }

    public void RemoveProduct(CartProduct cartProduct)
    {
        _context.CartProducts.Remove(cartProduct);
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}
