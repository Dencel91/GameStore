using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService.Data;
using UserService.DataServices.MessageBus;
using UserService.DataServices.MessageBus.EventProcessing;
using UserService.Extensions;
using UserService.Services;
using UserService.SyncData.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient<IProductDataClient, ProductDataClient>();

builder.AddSqlDatabase();
builder.AddAuthentication();

builder.Environment.IsDevelopment();
builder.Services.AddScoped<IUserRepo, UserRepo>();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Development");

DbPreparation.Population(app);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();