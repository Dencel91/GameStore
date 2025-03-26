using Microsoft.EntityFrameworkCore;
using CartService.Data;
using Microsoft.Extensions.Configuration;
using Grpc.Net.Client;
using System.Threading.Channels;
using ProductService;
using System.Runtime.InteropServices;

namespace CartService.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddSqlDatabase(this WebApplicationBuilder builder)
    {
        //var userServiceConnection = "";
        //if (builder.Environment.IsDevelopment())
        {
            //userServiceConnection = builder.Configuration.GetConnectionString("UserServiceConnection");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMem"));

            builder.Services.AddScoped<ICartRepo, CartRepo>();
            builder.Services.AddScoped<ICartProductRepo, CartProductRepo>();
            return;
        }
        //else
        //{
        //    userServiceConnection = Environment.GetEnvironmentVariable("UserServiceConnection");
        //}

        //builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(userServiceConnection));
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

    public static void AddLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            builder.Logging.AddEventLog(settings => settings.SourceName = "Cart Service");
        }
    }
}
