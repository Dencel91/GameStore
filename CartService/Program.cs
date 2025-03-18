using CartService.DataServices;
using CartService.DataServices.Grpc;
using CartService.Extensions;
using CartService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddSqlDatabase();

builder.Services.AddScoped<ICartService, CartService.Services.CartService>();
builder.Services.AddScoped<IProductDataClient, ProductDataClient>();

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
