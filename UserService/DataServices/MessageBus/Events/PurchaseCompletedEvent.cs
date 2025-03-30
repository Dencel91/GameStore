namespace UserService.DataServices.MessageBus.Events;

public class PurchaseCompletedEvent : Event
{
    public Guid UserId { get; set; }

    public IEnumerable<int> ProductIds { get; set; } = [];

    public PurchaseCompletedEvent()
    {
        EventTypeName = "Purchase completed";
    }
}
