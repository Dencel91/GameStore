using UserService.Models;

namespace UserService.Data;

public interface IUserRepo
{
    bool SaveChanges();

    IEnumerable<User> GetAllUsers();

    User GetUserById(int id);

    Task CreateUser(User user);
}
