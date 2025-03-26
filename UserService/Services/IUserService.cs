using UserService.Dtos;
using UserService.Models;

namespace UserService.Services;

public interface IUserService
{
    Task<User> GetUserById(int id);
    Task<User> CreateUser(CreateUserRequest createUserRequest);
}
