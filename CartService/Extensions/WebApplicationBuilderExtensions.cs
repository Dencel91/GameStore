using Microsoft.EntityFrameworkCore;
using CartService.Data;
using ProductService;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService;

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

        var userServiceAddress = builder.Configuration["GrpcConfigs:UserServiceUrl"];

        if (string.IsNullOrEmpty(userServiceAddress))
        {
            throw new InvalidOperationException("GrpcConfigs:UsertServiceUrl is empty");
        }

        builder.Services.AddGrpcClient<GrpcUser.GrpcUserClient>(o =>
        {
            o.Address = new Uri(userServiceAddress);
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

    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        string signingKey = (builder.Environment.IsDevelopment()
            ? builder.Configuration["Authentication:Key"]
            : Environment.GetEnvironmentVariable("AuthenticationKey"))
            ?? throw new InvalidOperationException("No authentication key");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
            options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Authentication:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Authentication:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ValidateLifetime = true,
                };
            }
        );
    }
}
