using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductService.Data;
using System.Text;

namespace ProductService.Extensions;

public static class WebApplicationBuilderExtensions
{
    private const string ProductServiceConnectioName = "ProductServiceConnection";
    public static void AddSqlDatabase(this WebApplicationBuilder builder)
    {
        var productServiceConnection = "";

        if (builder.Environment.IsDevelopment())
        {
            productServiceConnection = builder.Configuration.GetConnectionString(ProductServiceConnectioName);
        }
        else
        {
            productServiceConnection = Environment.GetEnvironmentVariable(ProductServiceConnectioName);
        }

        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(productServiceConnection));
    }
}
