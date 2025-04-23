using GameStore.Common.Constants;
using GameStore.Common.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.InteropServices;
using System.Text;

namespace GameStore.Common.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        string signingKey = ConfigHelper.GetSecret(builder.Environment, builder.Configuration, "Authentication-Key");

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

    public static void AddCorsPolicy(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicies.Development, policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }
        else
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicies.Production, policy =>
                {
                    policy.WithOrigins("https://www.game-store-dencel.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }
    }

    public static void AddLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            builder.Logging.AddEventLog(settings => settings.SourceName = builder.Configuration["ProjectName"]);
        }
    }

    public static void AddGrpcClient<T>(this WebApplicationBuilder builder, string urlConfigName) where T : class
    {
        var serviceAddress = builder.Configuration[urlConfigName];

        if (string.IsNullOrEmpty(serviceAddress))
        {
            throw new InvalidOperationException($"Grpc congif error: serviceUrl at {urlConfigName} is empty");
        }

        builder.Services.AddGrpcClient<T>(o =>
        {
            o.Address = new Uri(serviceAddress);
        });
    }
}
