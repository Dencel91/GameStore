namespace UserService.DataServices.MessageBus.Events;

public class UserRegisteredEvent : Event
{
    public Guid UserId { get; set; }

    public required string UserName { get; set; }

    public UserRegisteredEvent()
    {
        EventTypeName = "User registered";
    }
}
