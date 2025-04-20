using UserService.Dtos;
using UserService.Models;

namespace UserService.Services;

public interface IUserService
{
    Task<User> GetCurrentUser();

    Task<User?> GetUserById(Guid id);

    Task<User> AddUser(AddUserRequest createUserRequest);

    Task<IEnumerable<ProductDto>> GetCurrentUserProducts();

    Task<IEnumerable<ProductDto>> GetUserProducts(Guid userId);

    Task AddProductsToUser(Guid userId, IEnumerable<int> productIds);

    Task<GetUserProductInfoResponse> GetUserProductInfo(int productId);

    Task AddFreeProductToUser(int productId);
}
