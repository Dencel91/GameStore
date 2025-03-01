using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductService.Models;

namespace ProductService.Data;

public static class DbPreparation
{
    public static void Population(IApplicationBuilder app, bool isDevelopment)
    {
        using ( var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetRequiredService<AppDbContext>(), isDevelopment);
        }
    }

    private static void SeedData(AppDbContext context, bool isDevelopment)
    {
        if (!isDevelopment)
        {
            Console.WriteLine("Attempting to aooly migration.");

            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration failed: {ex.Message}");
            }
        }

        if (context.Products.Any())
        {
            return;
        }

        context.Products.AddRange(
            new Product() { Name = "Doka 2", Description = "Pretty mean game.", Price = 0D },
            new Product() { Name = "Strike", Description = "Pretty easy game.", Price = 20D },
            new Product() { Name = "Craft", Description = "Pretty peaceful game.", Price = 50D }
        );

        context.ProductToUsers.AddRange(
            new ProductToUser() { ProductId = 1, UserId = 1 },
            new ProductToUser() { ProductId = 2, UserId = 1 },
            new ProductToUser() { ProductId = 3, UserId = 1 },
            new ProductToUser() { ProductId = 1, UserId = 2 }
        );

        context.SaveChanges();
    }
}
