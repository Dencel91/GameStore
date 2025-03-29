using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
