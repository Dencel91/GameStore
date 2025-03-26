using AuthService.Dtos;
using AuthService.Models;

namespace AuthService.Services;

public interface IAuthService
{
    Task<User> GetUserById(int id);
    Task<User> CreateUser(CreateUserRequest createUserRequest);
}
