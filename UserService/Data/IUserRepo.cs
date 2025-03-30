using UserService.Models;

namespace UserService.Data;

public interface IUserRepo
{
    Task<bool> SaveChanges();

    IEnumerable<User> GetAllUsers();

    Task<User?> GetUserById(Guid id);

    Task CreateUser(User user);

    Task<IEnumerable<int>> GetUserProducts(Guid UserId);

    Task AddProductToUser(Guid userId, IEnumerable<int> productIds);
}
