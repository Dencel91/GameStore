using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public class UserRepo(AppDbContext context) : IUserRepo
    {
        public async Task<bool> SaveChanges()
        {
            var updated = await context.SaveChangesAsync() > 0;
            return updated;
        }

        public Task<User?> GetUserById(Guid userId)
        {
            return context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public Task<User?> GetUserByName(string userName)
        {
            return context.Users.FirstOrDefaultAsync(u => u.Name.Equals(userName));
        }

        public Task<User?> GetUserByEmail(string email)
        {
            return context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        }

        public async Task AddUser(User user)
        {
            await context.Users.AddAsync(user);
        }

        public Task<bool> UserExists(string userName)
        {
            return context.Users.AnyAsync(u => u.Name == userName);
        }
    }
}
