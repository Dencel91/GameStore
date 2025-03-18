using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.DataServices;
using ProductService.DataServices.Grpc;
using ProductService.EventProcessing;
using ProductService.Extensions;
using ProductService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddSqlDatabase();

builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IProductToUserRepo, ProductToUserRepo>();
builder.Services.AddScoped<IProductService, ProductService.Services.ProductService>();

builder.Services.AddGrpc();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcProductService>();

// TODO: Fix
app.MapGet("/protos/products.proto", () =>
{
    var file = Path.Combine(app.Environment.ContentRootPath, "Protos", "products.proto");
    return Results.File("Protos/products.proto", "text /plain");
});

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

DbPreparation.Population(app, app.Environment.IsDevelopment());

app.Run();