using CartService.DataServices;
using CartService.DataServices.Grpc;
using CartService.Extensions;
using CartService.Services;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddSqlDatabase();
builder.AddGrpcClients();
builder.AddLogging();

builder.Services.AddScoped<ICartService, CartService.Services.CartService>();
builder.Services.AddScoped<IProductDataClient, ProductDataClient>();

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddCors(options =>
{
    // this defines a CORS policy called "default"
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
    app.UseCors("Development");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
