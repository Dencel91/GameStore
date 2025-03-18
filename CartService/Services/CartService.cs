using Microsoft.AspNetCore.Http.HttpResults;
using CartService.Data;
using CartService.DataServices;
using CartService.Events;
using CartService.Models;
using CartService.DataServices.Grpc;

namespace CartService.Services;

public class CartService : ICartService
{
    private readonly ICartRepo _cartRepo;
    private readonly ICartProductRepo _cartProductRepo;
    private readonly IMessageBusClient _messageBusClient;
    private readonly IProductDataClient _productDataClient;

    public CartService(
        ICartRepo cartRepo,
        ICartProductRepo cartProductRepo,
        IMessageBusClient messageBusClient,
        IProductDataClient productDataClient)
    {
        _cartRepo = cartRepo;
        _cartProductRepo = cartProductRepo;
        _messageBusClient = messageBusClient;
        _productDataClient = productDataClient;
    }

    public async Task<Cart> GetCartById(int id)
    {
        var cart = await _cartRepo.GetCartById(id);

        if (cart == null)
        {
            return cart;
        }

        var cartProducts = await _cartProductRepo.GetProductsByCartId(id);

        // TODO: call ProductService to get product info
        var products = new List<Product>();
        cart.Products = products;

        foreach (var cartProduct in cartProducts)
        {
            //TODO: call one request with many ids
            var product = _productDataClient.GetProductById(cartProduct.ProductId);
            products.Add(product);
        }

        foreach(var product in cart.Products)
        {
            cart.TotalPrice += product.Price;
        }

        return cart;
    }

    public async Task<Cart> AddProduct(int cartId, int productId)
    {
        Cart cart;
        if (cartId == 0)
        {
            cart = await _cartRepo.CreateCart();
        }
        else
        {
            cart = await _cartRepo.GetCartById(cartId);
            if (cart is null)
            {
                throw new ArgumentException("Invalid cart id");
            }
        }

        ValidateProductId(productId);

        if (cart.UserId != 0)
        {
            ValidateUserProducts(cart.UserId, [productId]);
        }

        var cartProduct = new CartProduct
        {
            CartId = cart.Id,
            ProductId = productId
        };

        _cartProductRepo.AddProduct(cartProduct);

        _cartRepo.SaveChanges();

        return await this.GetCartById(cart.Id);
    }

    private void ValidateProductId(int productId)
    {
        ArgumentNullException.ThrowIfNull(productId);

        var product = _productDataClient.GetProductById(productId);
        if (product == null)
        {
            throw new ArgumentException("Invalid product id");
        }
    }

    private void ValidateUserProducts(int userId, IEnumerable<int> productIds)
    {
        var userProducts = _productDataClient.GetProductsByUserId(userId);

        var intersects = userProducts.Select(p => p.Id).Intersect(productIds);

        if (intersects.Any())
        {
            throw new ArgumentException("User already has these products");
        }
    }

    public async Task<Cart> RemoveProduct(int cartId, int productId)
    {
        ArgumentNullException.ThrowIfNull(cartId);
        ArgumentNullException.ThrowIfNull(productId);

        var cartExists = await _cartRepo.CartExists(cartId);
        if (!cartExists)
        {
            throw new ArgumentException("Invalid cart id");
        }

        var cartProduct = new CartProduct
        {
            CartId = cartId,
            ProductId = productId
        };

        _cartProductRepo.RemoveProduct(cartProduct);

        _cartRepo.SaveChanges();

        return await this.GetCartById(cartId);
    }
    public async Task StartPayment(int cartId)
    {
        ArgumentNullException.ThrowIfNull(cartId);

        var cart = await this.GetCartById(cartId);

        if (cart == null)
        {
            throw new ArgumentException("Cart not found");
        }

        if (cart.UserId == 0)
        {
            //TODO redirect to auth page
            cart.UserId = 1;
        }

        ValidateUserProducts(cart.UserId, cart.Products.Select(p => p.Id));

        _cartRepo.SaveChanges();
    }

    public async Task CompletePayment(int cartId)
    {
        ArgumentNullException.ThrowIfNull(nameof(cartId));

        var cart = await this.GetCartById(cartId);
        //TODO send message to some service than user has bought products
        var purchaseCompletedEvent = new PurchaseCompletedEvent()
        {
            UserId = cart.UserId,
            Products = cart.Products,
        };

        _messageBusClient.PublishPurchaseCompleted(purchaseCompletedEvent);
    }
}
