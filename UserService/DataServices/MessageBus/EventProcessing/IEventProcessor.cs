namespace UserService.DataServices.MessageBus.EventProcessing;

public interface IEventProcessor
{
    Task ProcessEvent(string message);
}
