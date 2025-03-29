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
