using AuthService.Data;
using AuthService.Extensions;
using AuthService.Services;
using AuthService.SyncData.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient<IProductDataClient, ProductDataClient>();

builder.AddSqlDatabase();

builder.Environment.IsDevelopment();
builder.Services.AddScoped<IUserRepo, UserRepo>();

builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();   

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