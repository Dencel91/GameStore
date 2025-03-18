using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CartService.Models;

namespace CartService.Data;

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
            //context.Database.Migrate();

            context.SaveChanges();
        }
        catch (Exception)
        {

            throw;
        }
    }
}
