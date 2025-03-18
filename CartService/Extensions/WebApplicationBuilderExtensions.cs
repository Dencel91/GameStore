using Microsoft.EntityFrameworkCore;
using CartService.Data;

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
}
