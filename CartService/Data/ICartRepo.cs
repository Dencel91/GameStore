using CartService.Models;

namespace CartService.Data;

public interface ICartRepo
{
    Task<bool> SaveChanges();

    Task<Cart?> GetCartById(int id);

    Task<Cart> CreateCart();

    void DeleteCart(Cart cart);

    Task<bool> CartExists(int id);

    Task<Cart?> GetCartByUserId(Guid userId);
}
