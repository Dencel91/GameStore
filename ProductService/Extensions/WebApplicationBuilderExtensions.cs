using GameStore.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;

namespace ProductService.Extensions;

public static class WebApplicationBuilderExtensions
{
    private const string ProductServiceConnectioName = "ProductServiceConnection";
    public static void AddSqlDatabase(this WebApplicationBuilder builder)
    {
        var productServiceConnection = 
            ConfigHelper.GetSecret(builder.Environment, builder.Configuration, ProductServiceConnectioName);

        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(productServiceConnection));
    }
}
