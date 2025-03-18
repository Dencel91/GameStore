using Microsoft.EntityFrameworkCore;
using UserService.Data;

namespace UserService.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddSqlDatabase(this WebApplicationBuilder builder)
    {
        var userServiceConnection = "";
        if (builder.Environment.IsDevelopment())
        {
            userServiceConnection = builder.Configuration.GetConnectionString("UserServiceConnection");
        }
        else
        {
            userServiceConnection = Environment.GetEnvironmentVariable("UserServiceConnection");
        }

        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(userServiceConnection));
    }
}
