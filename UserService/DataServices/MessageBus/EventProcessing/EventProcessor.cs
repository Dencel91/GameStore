using System.Text.Json;
using UserService.DataServices.MessageBus.Events;
using UserService.Dtos;
using UserService.Models;
using UserService.Services;

namespace UserService.DataServices.MessageBus.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EventProcessor> _logger;

    public EventProcessor(
        IServiceScopeFactory scopeFactory,
        ILogger<EventProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    private static EventTypes DeterminateEvent(string message)
    {
        var publishedEvent = JsonSerializer.Deserialize<Event>(message) ??
            throw new ArgumentException("Can not deserialize a message", nameof(message));

        switch (publishedEvent.EventTypeName)
        {
            case "User registered":
                return EventTypes.UserRegistered;
            case "Purchase completed":
                return EventTypes.PurchaseCompleted;
            default:
                return EventTypes.Undetermened;
        }
    }

    public async Task ProcessEvent(string message)
    {
        try
        {
            var eventType = DeterminateEvent(message);

            switch (eventType)
            {
                case EventTypes.UserRegistered:
                    await AddUser(message);
                    break;
                case EventTypes.PurchaseCompleted:
                    await AddProductsToUser(message);
                    break;
                default:
                    _logger.LogError("Event is not recognized: {message}", message);
                    throw new ArgumentException($"Event is not recognized: {message}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Process event error: {error}. message: {message}", ex.Message, message);
            throw;
        }

    }

    private Task<User> AddUser(string message)
    {
        var userRegisteredEvent = JsonSerializer.Deserialize<UserRegisteredEvent>(message) ??
            throw new ArgumentException("Can not deserialize a message", nameof(message));

        using var scope = _scopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        try
        {
            var request = new AddUserRequest()
            {
                Id = userRegisteredEvent.UserId,
                Name = userRegisteredEvent.UserName,
                Email = userRegisteredEvent.Email
            };

            return userService.AddUser(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Could not add user. Message: {message}. Exception message: {exceptionMessage}",
                message,
                ex.Message);

            throw;
        }
    }

    private async Task AddProductsToUser(string message)
    {
        var purchaseCompletedEvent = JsonSerializer.Deserialize<PurchaseCompletedEvent>(message) ??
            throw new ArgumentException("Can not deserialize a message", nameof(message));

        using var scope = _scopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        try
        {
            await userService.AddProductsToUser(purchaseCompletedEvent.UserId, purchaseCompletedEvent.ProductIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Could not add products to user. Message: {message}. Exception message: {exceptionMessage}",
                message,
                ex.Message);

            throw;
        }
    }
}
