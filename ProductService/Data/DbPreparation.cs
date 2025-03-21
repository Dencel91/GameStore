using Microsoft.EntityFrameworkCore;

namespace ProductService.Data;

public static class DbPreparation
{
    public static void Migration(IApplicationBuilder app, bool isDevelopment)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();

        try
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration failed: {ex.Message}");
        }
    }
}
