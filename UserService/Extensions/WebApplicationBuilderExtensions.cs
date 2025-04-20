using GameStore.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using UserService.Data;

namespace UserService.Extensions;

public static class WebApplicationBuilderExtensions
{
    private const string UserServiceConnectionConfig = "UserServiceConnection";
    public static void AddSqlDatabase(this WebApplicationBuilder builder)
    {
        var serviceConnection =
            ConfigHelper.GetSecret(builder.Environment, builder.Configuration, UserServiceConnectionConfig);

        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(serviceConnection));
    }
}
