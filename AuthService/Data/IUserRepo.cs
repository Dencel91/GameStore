using AuthService.Models;

namespace AuthService.Data
{
    public interface IUserRepo
    {
        Task<bool> SaveChanges();

        Task<User?> GetUserById(Guid userId);

        Task<User?> GetUserByName(string userName);

        Task AddUser(User user);

        Task<bool> UserExists(string userName);
        
    }
}
