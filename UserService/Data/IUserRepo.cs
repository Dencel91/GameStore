using UserService.Models;

namespace UserService.Data;

public interface IUserRepo
{
    bool SaveChanges();

    IEnumerable<User> GetAllUsers();

    Task<User?> GetUserById(Guid id);

    Task CreateUser(User user);
}
