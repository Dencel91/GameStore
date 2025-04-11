using ProductService.Data;
using ProductService.DataServices;
using ProductService.DataServices.Grpc;
using ProductService.EventProcessing;
using ProductService.Extensions;
using ProductService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddSqlDatabase();
builder.AddAuthentication();

builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IProductImageRepo, ProductImageRepo>();
builder.Services.AddScoped<IProductReviewRepo, ProductReviewRepo>();

builder.Services.AddTransient(typeof(Lazy<>), typeof(LazyResolver<>));
builder.Services.AddScoped<IProductService, ProductService.Services.ProductService>();
builder.Services.AddScoped<IFileService, AzureFileService>();

builder.Services.AddGrpc();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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

DbPreparation.Migration(app, app.Environment.IsDevelopment());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Development");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

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
