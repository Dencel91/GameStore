using CartService.Data;
using CartService.DataServices;
using CartService.DataServices.Grpc;
using CartService.Services;
using GameStore.Common.Constants;
using GameStore.Common.Extensions;
using GameStore.Common.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ProductService;
using UserService;

var builder = WebApplication.CreateBuilder(args);

builder.AddLogging();
builder.AddAuthentication();

builder.AddGrpcClient<GrpcProduct.GrpcProductClient>("GrpcConfigs:ProductServiceUrl");
builder.AddGrpcClient<GrpcUser.GrpcUserClient>("GrpcConfigs:UserServiceUrl");

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMem"));

builder.Services.AddScoped<ICartRepo, CartRepo>();

builder.Services.AddScoped<ICartService, CartService.Services.CartService>();
builder.Services.AddScoped<IProductDataClient, ProductDataClient>();
builder.Services.AddScoped<IUserDataClient, UserDataClient>();

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.AddCorsPolicy();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(CorsPolicies.Development);
}
else
{
    app.UseCors(CorsPolicies.Production);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();
