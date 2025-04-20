using AuthService.Data;
using AuthService.DataServices;
using AuthService.Extensions;
using AuthService.Services;
using GameStore.Common.Constants;
using GameStore.Common.Extensions;
using GameStore.Common.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddLogging();
builder.AddSqlDatabase();

builder.Services.AddScoped<IUserRepo, UserRepo>();

builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.AddCorsPolicy();

var app = builder.Build();

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
