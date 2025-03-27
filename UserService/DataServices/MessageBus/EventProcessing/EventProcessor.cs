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
            default:
                return EventTypes.Undetermened;
        }
    }

    public async Task ProcessEvent(string message)
    {
        var eventType = DeterminateEvent(message);
        switch (eventType)
        {
            case EventTypes.UserRegistered:
                await AddUser(message);
                break;
            default:
                _logger.LogError("Event is not recognized: {message}", message);
                throw new ArgumentException($"Event is not recognized: {message}");
        }
    }

    private Task<User> AddUser(string message)
    {
        using var scope = _scopeFactory.CreateScope();

        var userRegisteredEvent = JsonSerializer.Deserialize<UserRegisteredEvent>(message) ??
            throw new ArgumentException("Can not deserialize a message", nameof(message));

        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        try
        {
            var request = new AddUserRequest()
            {
                Id = userRegisteredEvent.UserId,
                Name = userRegisteredEvent.UserName,
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
}
