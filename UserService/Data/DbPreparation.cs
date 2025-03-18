using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Models;

namespace UserService.Data;

public static class DbPreparation
{
    public static void Population(IApplicationBuilder app)
    {
        using(var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetRequiredService<AppDbContext>());
        }
    }

    private static void SeedData(AppDbContext context)
    {
        try
        {
            context.Database.Migrate();

            if (context.Users.Any())
            {
                return;
            }

            context.Users.AddRange(
                new User() { UID = "123124132", Name = "Dencel", Email = "Dencel@gmail.com", Password = "test" },
                new User() { UID = "", Name = "Brak0ne", Email = "Dencel@gmail.com", Password = "test" },
                new User() { UID = "", Name = "Foxy", Email = "Dencel@gmail.com", Password = "test" }
            );

            context.SaveChanges();
        }
        catch (Exception)
        {

            throw;
        }
    }
}
