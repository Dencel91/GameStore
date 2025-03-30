using CartService.DTOs;

namespace CartService.Services;

public interface ICartService
{
    Task<CartDto?> GetCartById(int id);

    Task<CartDto> AddProduct(int cartId, int productId);

    Task<CartDto> RemoveProduct(int cartId, int productId);

    Task<CartDto> MergeCarts(int cartId);

    Task<CartDto?> GetCurrentUserCart();

    Task StartPayment(int cartId);

    Task CompletePayment(int cartId);
}
