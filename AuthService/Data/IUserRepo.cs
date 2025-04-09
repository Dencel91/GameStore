using AuthService.Models;

namespace AuthService.Data
{
    public interface IUserRepo
    {
        Task<bool> SaveChanges();

        Task<User?> GetUserById(Guid userId);

        Task<User?> GetUserByName(string userName);

        Task<User?> GetUserByEmail(string email);

        Task AddUser(User user);

        Task<bool> UserNameExists(string userName);

        Task<bool> EmailExists(string email);

    }
}
