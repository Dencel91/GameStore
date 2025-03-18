using CartService.Models;

namespace CartService.Data;

public interface ICartRepo
{
    bool SaveChanges();

    Task<Cart> GetCartById(int id);

    Task<Cart> CreateCart();

    Task<bool> CartExists(int id);
}
