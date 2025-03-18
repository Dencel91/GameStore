using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProductService.Data;
using ProductService.Events;
using ProductService.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(
        IServiceScopeFactory scopeFactory,
        IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }

    private EventTypes DeterminateEvent(string message)
    {
        Console.WriteLine("Determining event type");

        var publishedEvent = JsonSerializer.Deserialize<Event>(message);

        switch (publishedEvent.EventTypeName)
        {
            case "Purchase completed":
                Console.WriteLine("Purchase completed event detected");
                return EventTypes.PurchaseCompleted;
            default:
                return EventTypes.Undetermened;
        }
    }

    public Task ProcessEvent(string message)
    {
        var eventType = DeterminateEvent(message);
        switch (eventType)
        {
            case EventTypes.PurchaseCompleted:
                this.AddProductsToUser(message);
                break;
            default:
                Console.WriteLine("Event not recognized");
                break;
        }

        return Task.CompletedTask;
    }

    private void AddProductsToUser(string message)
    {
        using var scope = _scopeFactory.CreateScope();

        var purchaseCompletedEvent = JsonSerializer.Deserialize<PurchaseCompletedEvent>(message);

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            foreach (var product in purchaseCompletedEvent.Products)
            {
                context.ProductToUsers.Add(new ProductToUser { ProductId = product.Id, UserId = purchaseCompletedEvent.UserId });
                Console.WriteLine($"Added product {product.Id} to user {purchaseCompletedEvent.UserId}");
            }

            context.SaveChanges();
        }
        catch (Exception)
        {
            Console.WriteLine("Could not add products to user");
        }
        
    }
}


