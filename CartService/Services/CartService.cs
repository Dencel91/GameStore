using CartService.Data;
using CartService.DataServices;
using CartService.Events;
using CartService.Models;
using CartService.DataServices.Grpc;
using System.Security.Claims;
using AutoMapper;
using CartService.DTOs;

namespace CartService.Services;

public class CartService : ICartService
{
    private readonly ICartRepo _cartRepo;
    private readonly IMessageBusClient _messageBusClient;
    private readonly IProductDataClient _productDataClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserDataClient _userDataClient;
    private readonly IMapper _mapper;

    public CartService(
        ICartRepo cartRepo,
        IMessageBusClient messageBusClient,
        IProductDataClient productDataClient,
        IHttpContextAccessor httpContextAccessor,
        IUserDataClient userDataClient,
        IMapper mapper)
    {
        _cartRepo = cartRepo;
        _messageBusClient = messageBusClient;
        _productDataClient = productDataClient;
        _httpContextAccessor = httpContextAccessor;
        _userDataClient = userDataClient;
        _mapper = mapper;
    }

    public async Task<CartDto?> GetCartById(int id)
    {
        var cart = await _cartRepo.GetCartById(id);

        if (cart is null)
        {
            return null;
        }

        var cartDto = _mapper.Map<CartDto>(cart);

        var cartProductIds = cart.Products.Select(cp => cp.ProductId);
        cartDto.Products = _productDataClient.GetProductsByIds(cartProductIds);

        cartDto.TotalPrice = cartDto.Products.Sum(p => p.Price);

        return cartDto;
    }

    public async Task<CartDto> AddProduct(int cartId, int productId)
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

            if (cart.Products.Select(p => p.ProductId).Contains(productId))
            {
                throw new ArgumentException("Product already in cart");
            }
        }

        if (cart.UserId == Guid.Empty)
        {
            var userId = GetCurrentUserId();
            if (userId != Guid.Empty)
            {
                cart.UserId = userId;
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

        cart.Products.Add(cartProduct);

        await _cartRepo.SaveChanges();

        return await this.GetCartById(cart.Id);
    }

    private void ValidateProductId(int productId)
    {
        if (productId == 0)
        {
            throw new ArgumentException("Invalid product id");
        }

        var product = _productDataClient.GetProductById(productId);
        if (product == null)
        {
            throw new ArgumentException("Invalid product id");
        }
    }

    private void ValidateUserProducts(Guid userId, IEnumerable<int> productIds)
    {
        var userProducts = _userDataClient.GetUserProducts(userId);

        var intersects = userProducts.Select(p => p.Id).Intersect(productIds);

        if (intersects.Any())
        {
            throw new ArgumentException("User already has these products");
        }
    }

    public async Task<CartDto> RemoveProduct(int cartId, int productId)
    {
        if (cartId == 0)
        {
            throw new ArgumentException("Invalid cart id", nameof(cartId));
        }

        if (productId == 0)
        {
            throw new ArgumentException("Invalid product id", nameof(productId));
        }

        var cart = await _cartRepo.GetCartById(cartId);

        if (cart is null)
        {
            throw new ArgumentException("Invalid cart id", nameof(cartId));
        }

        var product = cart.Products.FirstOrDefault(p => p.ProductId == productId);

        if (product is null)
        {
            throw new ArgumentException("Invalid product id", nameof(productId));
        }

        cart.Products.Remove(product);

        await _cartRepo.SaveChanges();

        return await this.GetCartById(cartId);
    }

    public async Task<CartDto> MergeCarts(int cartId)
    {
        if (cartId == 0)
        {
            throw new ArgumentException("Invalid cart id", nameof(cartId));
        }

        var cart = await _cartRepo.GetCartById(cartId) ?? throw new ArgumentException("Cart not found");

        var userId = GetCurrentUserId();
        var userCart = await _cartRepo.GetCartByUserId(userId);

        if (userCart is null)
        {
            cart.UserId = userId;
            await _cartRepo.SaveChanges();
            return await this.GetCartById(cartId);
        }

        var userProducts = _userDataClient.GetUserProducts(userId);

        var newProductIds = cart.Products.Select(cp => cp.ProductId)
            .Except(userProducts.Select(p => p.Id))
            .Except(userCart.Products.Select(cp => cp.ProductId));

        foreach (var productId in newProductIds)
        {
            userCart.Products.Add(new CartProduct() { CartId = userCart.Id, ProductId = productId });
        }

        _cartRepo.DeleteCart(cart);

        await _cartRepo.SaveChanges();

        return await this.GetCartById(userCart.Id);
    }

    public async Task<CartDto?> GetCurrentUserCart()
    {
        var userId = GetCurrentUserId();

        if (userId == Guid.Empty)
        {
            return null;
        }

        var cart = await _cartRepo.GetCartByUserId(userId);

        if (cart is null)
        {
            return null;
        }

        return await GetCartById(cart.Id);
    }

    public async Task StartPayment(int cartId)
    {
        if (cartId == 0)
        {
            throw new ArgumentException("Invalid cart id", nameof(cartId));
        }

        var cart = await this.GetCartById(cartId);

        if (cart == null)
        {
            throw new ArgumentException("Cart not found");
        }

        if (cart.UserId == Guid.Empty)
        {
            cart.UserId = GetCurrentUserId();
        }

        ValidateUserProducts(cart.UserId, cart.Products.Select(p => p.Id));

        await _cartRepo.SaveChanges();
    }

    private Guid GetCurrentUserId()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            return Guid.Empty;
        }

        //var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        string userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        return string.IsNullOrEmpty(userId) ? Guid.Empty : Guid.Parse(userId);
    }

    public async Task CompletePayment(int cartId)
    {
        if (cartId == 0)
        {
            throw new ArgumentException("Invalid cart id", nameof(cartId));
        }

        var cart = await this.GetCartById(cartId);

        var purchaseCompletedEvent = new PurchaseCompletedEvent()
        {
            UserId = cart.UserId,
            ProductIds = cart.Products.Select(p => p.Id),
        };

        _messageBusClient.PublishPurchaseCompleted(purchaseCompletedEvent);
    }
}
