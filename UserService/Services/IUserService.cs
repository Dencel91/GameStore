using UserService.Dtos;
using UserService.Models;

namespace UserService.Services;

public interface IUserService
{
    Task<User> GetCurrentUserInfo();

    Task<User> GetUserById(Guid id);

    Task<User> AddUser(AddUserRequest createUserRequest);
}
