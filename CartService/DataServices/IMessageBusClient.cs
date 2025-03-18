using CartService.Events;

namespace CartService.DataServices
{
    public interface IMessageBusClient
    {
        Task PublishPurchaseCompleted(PurchaseCompletedEvent purchaseCompletedEvent);
    }
}
