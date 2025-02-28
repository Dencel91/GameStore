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
        if (context.Users.Any())
        {
            return;
        }

        context.Users.AddRange(
            new User() { UID = "", Name = "Dencel"},
            new User() { UID = "", Name = "Brak0ne"},
            new User() { UID = "", Name = "Foxy" }
        );


        context.SaveChanges();
    }
}
