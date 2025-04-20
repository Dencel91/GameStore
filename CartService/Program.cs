using CartService.DataServices;
using CartService.DataServices.Grpc;
using CartService.Extensions;
using CartService.Services;
using GameStore.Common.Constants;
using GameStore.Common.Extensions;
using GameStore.Common.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddSqlDatabase();
builder.AddGrpcClients();
builder.AddLogging();
builder.AddAuthentication();

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
