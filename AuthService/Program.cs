using AuthService.Data;
using AuthService.Extensions;
using AuthService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddSqlDatabase();
builder.Services.AddScoped<IUserRepo, UserRepo>();

builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
