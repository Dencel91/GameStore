using System.Security.Claims;
using UserService.Data;
using UserService.DataServices.Grpc;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IProductDataClient _productClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepo userRepo, IProductDataClient productClient, IHttpContextAccessor httpContextAccessor)
        {
            _userRepo = userRepo;
            _productClient = productClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User> GetCurrentUser()
        {
            var userId = GetCurrentUserId();

            var user = await _userRepo.GetUserById(userId)
                ?? throw new InvalidOperationException($"Current user not found: {userId}");

            return user;
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

        public async Task<User?> GetUserById(Guid id)
        {
            var user = await _userRepo.GetUserById(id);

            return user;
        }

        public async Task<User> AddUser(AddUserRequest createUserRequest)
        {
            var user = new User
            {
                Id = createUserRequest.Id,
                Name = createUserRequest.Name
            };

            await _userRepo.CreateUser(user);
            _userRepo.SaveChanges();

            return user;
        }

        public Task<IEnumerable<ProductDto>> GetCurrentUserProducts()
        {
            var userId = GetCurrentUserId();
            return GetUserProducts(userId);
        }

        public async Task<IEnumerable<ProductDto>> GetUserProducts(Guid userId)
        {
            var productIds = await _userRepo.GetUserProducts(userId);

            var products = _productClient.GetProductsByIds(productIds);

            return products;
        }

        public async Task AddProductsToUser(Guid userId, IEnumerable<int> productIds)
        {
            await _userRepo.AddProductToUser(userId, productIds);
            await _userRepo.SaveChanges();
        }

        public async Task<GetUserProductInfoResponse> GetUserProductInfo(int productId)
        {
            var response = new GetUserProductInfoResponse();

            var userId = GetCurrentUserId();

            var products = await _userRepo.GetUserProducts(userId);

            response.Owned = products.Contains(productId);

            return response;
        }

        public async Task AddFreeProductToUser(int productId)
        {
            var userId = GetCurrentUserId();

            await ValidateAddFreeProductToUserRequest(userId, productId);

            await AddProductsToUser(userId, [productId]);
        }

        private async Task ValidateAddFreeProductToUserRequest(Guid userId, int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Invalid product id");
            }

            var product = _productClient.GetProductById(productId);

            if (product?.Price > 0)
            {
                throw new ArgumentException("Product is not free");
            }

            var products = await GetUserProducts(userId);

            if (products.Any(p => p.Id == productId))
            {
                throw new ArgumentException("The user already has this product");
            }
        }
    }
}
