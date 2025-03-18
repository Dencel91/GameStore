using CartService.Models;

namespace CartService.Services;

public interface ICartService
{
    Task<Cart> GetCartById(int id);

    Task<Cart> AddProduct(int cartId, int productId);

    Task<Cart> RemoveProduct(int cartId, int productId);

    Task StartPayment(int cartId);

    Task CompletePayment(int cartId);
}
