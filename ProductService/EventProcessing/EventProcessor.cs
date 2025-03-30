using ProductService.Data;
using ProductService.Events;
using ProductService.Models;
using System.Text.Json;

namespace ProductService.EventProcessing;

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
            default:
                return EventTypes.Undetermened;
        }
    }

    public async Task ProcessEvent(string message)
    {
        var eventType = DeterminateEvent(message);
        switch (eventType)
        {
            default:
                _logger.LogError("Event is not recognized: {message}", message);
                throw new ArgumentException($"Event is not recognized: {message}");
        }
    }
}
