using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class UserRepo : IUserRepo
{
    private readonly AppDbContext _context;

    public UserRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        await _context.Users.AddAsync(user);
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _context.Users.ToList();
    }

    public Task<User?> GetUserById(Guid id)
    {
        return _context.Users.FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<IEnumerable<int>> GetUserProducts(Guid UserId)
    {
        var productRequest = _context.UserProducts
            .Where(up => up.UserId == UserId)
            .Select(up => up.ProductId);

        var products = await productRequest.ToListAsync();

        return products;
    }

    public Task AddProductToUser(Guid userId, IEnumerable<int> productIds)
    {
        var userProducts = productIds.Select(productId => new UserProduct
        {
            UserId = userId,
            ProductId = productId
        });

        return _context.UserProducts.AddRangeAsync(userProducts);
    }

    public async Task<bool> SaveChanges()
    {
        var updated = await _context.SaveChangesAsync() >= 0;
        return updated;
    }
}
