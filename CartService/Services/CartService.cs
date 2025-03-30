using CartService.Data;
using CartService.DataServices;
using CartService.Events;
using CartService.Models;
using CartService.DataServices.Grpc;
using System.Security.Claims;

namespace CartService.Services;

public class CartService : ICartService
{
    private readonly ICartRepo _cartRepo;
    private readonly ICartProductRepo _cartProductRepo;
    private readonly IMessageBusClient _messageBusClient;
    private readonly IProductDataClient _productDataClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserDataClient _userDataClient;

    public CartService(
        ICartRepo cartRepo,
        ICartProductRepo cartProductRepo,
        IMessageBusClient messageBusClient,
        IProductDataClient productDataClient,
        IHttpContextAccessor httpContextAccessor,
        IUserDataClient userDataClient)
    {
        _cartRepo = cartRepo;
        _cartProductRepo = cartProductRepo;
        _messageBusClient = messageBusClient;
        _productDataClient = productDataClient;
        _httpContextAccessor = httpContextAccessor;
        _userDataClient = userDataClient;
    }

    public async Task<Cart?> GetCartById(int id)
    {
        var cart = await _cartRepo.GetCartById(id);

        if (cart is null)
        {
            return cart;
        }

        var cartProductIds = await _cartProductRepo.GetProductIdsByCartId(id);


        cart.Products = _productDataClient.GetProductsByIds(cartProductIds);

        foreach (var product in cart.Products)
        {
            cart.TotalPrice += product.Price;
        }

        return cart;
    }

    public async Task<Cart> AddProduct(int cartId, int productId)
    {
        ValidateProductId(productId);

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

            var addedProducts = await _cartProductRepo.GetProductIdsByCartId(cart.Id);

            if (addedProducts.Contains(productId))
            {
                throw new ArgumentException("Product already in cart");
            }
        }

        if (cart.UserId != Guid.Empty)
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

    private void ValidateUserProducts(Guid userId, IEnumerable<int> productIds)
    {
        //TODO: FIX IT
        var userProducts = _userDataClient.GetUserProducts(userId);

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

        if (cart.UserId == Guid.Empty)
        {
            //TODO redirect to auth page
            cart.UserId = GetCurrentUserId();
        }

        ValidateUserProducts(cart.UserId, cart.Products.Select(p => p.Id));

        _cartRepo.SaveChanges();
    }

    private Guid GetCurrentUserId()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            throw new InvalidOperationException("No HttpContext");
        }

        //var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        string userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("No Identifier found in claims");

        return Guid.Parse(userId);
    }

    public async Task CompletePayment(int cartId)
    {
        ArgumentNullException.ThrowIfNull(nameof(cartId));

        var cart = await this.GetCartById(cartId);

        var purchaseCompletedEvent = new PurchaseCompletedEvent()
        {
            UserId = cart.UserId,
            ProductIds = cart.Products.Select(p => p.Id),
        };

        _messageBusClient.PublishPurchaseCompleted(purchaseCompletedEvent);
    }
}
