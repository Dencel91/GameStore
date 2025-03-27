using AuthService.Events;

namespace AuthService.DataServices;

public interface IMessageBusClient
{
    Task PublishUserRegistered(UserRegisteredEvent purchaseCompletedEvent);
}
