using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<UserProduct> UserProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Products)
            .WithOne(ptu => ptu.User)
            .HasForeignKey(ptu => ptu.UserId);

        modelBuilder.Entity<UserProduct>().HasKey(ptu => new { ptu.UserId, ptu.ProductId });
    }
}
