using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using GameStore.Common.Helpers;

namespace AuthService.Extensions;

public static class WebApplicationBuilderExtensions
{
    private const string AuthServiceConnectioName = "AuthServiceConnection";
    public static void AddSqlDatabase(this WebApplicationBuilder builder)
    {
        var serviceConnection =
            ConfigHelper.GetSecret(builder.Environment, builder.Configuration, AuthServiceConnectioName);

        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(serviceConnection));
    }
}
