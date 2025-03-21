using CartService.Models;

namespace CartService.Data;

public interface ICartProductRepo
{
    bool SaveChanges();

    Task<IEnumerable<int>> GetProductIdsByCartId(int cartId);

    void AddProduct(CartProduct cartProduct);

    void RemoveProduct(CartProduct cartProduct);
}   
