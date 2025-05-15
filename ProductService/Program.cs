using GameStore.Common.Constants;
using GameStore.Common.Extensions;
using GameStore.Common.Infrastructure;
using ProductService.Data;
using ProductService.DataServices.Grpc;
using ProductService.EventProcessing;
using ProductService.Extensions;
using ProductService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddLogging();
builder.AddSqlDatabase();
builder.AddAuthentication();

builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IProductImageRepo, ProductImageRepo>();
builder.Services.AddScoped<IProductReviewRepo, ProductReviewRepo>();

builder.Services.AddTransient(typeof(Lazy<>), typeof(LazyResolver<>));
builder.Services.AddScoped<IProductService, ProductService.Services.ProductService>();
builder.Services.AddScoped<IFileService, AzureFileService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisConnection"];
});

builder.Services.AddGrpc();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

builder.Services.AddControllers();
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

app.MapControllers();

app.UseExceptionHandler();

app.MapGrpcService<GrpcProductService>();

app.MapGet("/protos/products.proto", () =>
{
    var file = Path.Combine(app.Environment.ContentRootPath, "Protos", "products.proto");
    return Results.File(file, "text/plain");
});

app.Run();


public class LazyResolver<T> : Lazy<T>
{
    public LazyResolver(IServiceProvider serviceProvider)
        : base(() => serviceProvider.GetRequiredService<T>())
    {
    }
}
