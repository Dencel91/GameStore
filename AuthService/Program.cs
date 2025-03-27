using AuthService.Data;
using AuthService.DataServices;
using AuthService.Extensions;
using AuthService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddSqlDatabase();
builder.Services.AddScoped<IUserRepo, UserRepo>();

builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Development");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
