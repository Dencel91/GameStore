using Microsoft.EntityFrameworkCore;
using CartService.Models;

namespace CartService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartProduct> CartProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Cart>()
            .HasMany(c => c.Products)
            .WithOne(cp => cp.Cart)
            .HasForeignKey(cp => cp.CartId);

        modelBuilder.Entity<CartProduct>()
            .HasKey(cp => new { cp.CartId, cp.ProductId });
    }
}
