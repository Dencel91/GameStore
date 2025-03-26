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
            case "Purchase completed":
                return EventTypes.PurchaseCompleted;
            default:
                return EventTypes.Undetermened;
        }
    }

    public async Task ProcessEvent(string message)
    {
        var eventType = DeterminateEvent(message);
        switch (eventType)
        {
            case EventTypes.PurchaseCompleted:
                await AddProductsToUser(message);
                break;
            default:
                _logger.LogError("Event is not recognized: {message}", message);
                throw new ArgumentException($"Event is not recognized: {message}");
        }
    }

    private async Task AddProductsToUser(string message)
    {
        using var scope = _scopeFactory.CreateScope();

        var purchaseCompletedEvent = JsonSerializer.Deserialize<PurchaseCompletedEvent>(message) ??
            throw new ArgumentException("Can not deserialize a message", nameof(message));

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            foreach (var product in purchaseCompletedEvent.Products)
            {
                await context.ProductToUsers.AddAsync(new ProductToUser { ProductId = product.Id, UserId = purchaseCompletedEvent.UserId });
                _logger.LogInformation("Product {productId} is added to user {userId}", product.Id, purchaseCompletedEvent.UserId);
            }

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Could not add products to user. Message: {message}. Exception message: {exceptionMessage}",
                message,
                ex.Message);
        }
    }
}
