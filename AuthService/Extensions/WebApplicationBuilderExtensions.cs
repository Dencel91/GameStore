using Microsoft.EntityFrameworkCore;
using AuthService.Data;

namespace AuthService.Extensions;

public static class WebApplicationBuilderExtensions
{
    private const string AuthServiceConnectioName = "AuthServiceConnection";
    public static void AddSqlDatabase(this WebApplicationBuilder builder)
    {
        var productServiceConnection = "";

        if (builder.Environment.IsDevelopment())
        {
            productServiceConnection = builder.Configuration.GetConnectionString(AuthServiceConnectioName);
        }
        else
        {
            productServiceConnection = Environment.GetEnvironmentVariable(AuthServiceConnectioName);
        }

        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(productServiceConnection));
    }
}
