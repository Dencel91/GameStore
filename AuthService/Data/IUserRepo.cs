using AuthService.Models;

namespace AuthService.Data;

public interface IUserRepo
{
    bool SaveChanges();

    IEnumerable<User> GetAllUsers();

    User GetUserById(int id);

    Task CreateUser(User user);
}
