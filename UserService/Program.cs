using UserService.Data;
using UserService.Extensions;
using UserService.Services;
using UserService.SyncData.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient<IProductDataClient, ProductDataClient>();

builder.AddSqlDatabase();

builder.Environment.IsDevelopment();
builder.Services.AddScoped<IUserRepo, UserRepo>();

builder.Services.AddScoped<IUserService, UserService.Services.UserService>();   

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

DbPreparation.Population(app);

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();