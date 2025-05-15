using GameStore.Common.Constants;
using GameStore.Common.Extensions;
using GameStore.Common.Infrastructure;
using Microsoft.Extensions.Options;
using ProductService;
using Swashbuckle.AspNetCore.SwaggerGen;
using UserService.Data;
using UserService.DataServices.Grpc;
using UserService.DataServices.MessageBus;
using UserService.DataServices.MessageBus.EventProcessing;
using UserService.Extensions;
using UserService.Services;
using UserService.Swagger;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.AddLogging();
builder.AddSqlDatabase();
builder.AddGrpcClient<GrpcProduct.GrpcProductClient>("GrpcConfigs:ProductServiceUrl");
builder.AddAuthentication();

builder.Environment.IsDevelopment();
builder.Services.AddScoped<IUserRepo, UserRepo>();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();
builder.Services.AddScoped<IProductDataClient, ProductDataClient>();

builder.Services.AddGrpc();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisConnection"];
});

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

app.MapGrpcService<GrpcUserService>();

app.MapGet("/protos/products.proto", () =>
{
    var file = Path.Combine(app.Environment.ContentRootPath, "Protos", "users.proto");
    return Results.File(file, "text/plain");
});

app.Run();