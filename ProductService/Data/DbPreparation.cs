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
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger>();
            logger.LogError("Migration failed: {message}", ex.Message);
            throw;
        }
    }
}
