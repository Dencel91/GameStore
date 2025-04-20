using Microsoft.EntityFrameworkCore;
using ProductService;
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

    public static void AddGrpcClients(this WebApplicationBuilder builder)
    {
        var productServiceAddress = builder.Configuration["GrpcConfigs:ProductServiceUrl"];

        if (string.IsNullOrEmpty(productServiceAddress))
        {
            throw new InvalidOperationException("GrpcConfigs:ProductServiceUrl is empty");
        }

        builder.Services.AddGrpcClient<GrpcProduct.GrpcProductClient>(o =>
        {
            o.Address = new Uri(productServiceAddress);
        });
    }
}
