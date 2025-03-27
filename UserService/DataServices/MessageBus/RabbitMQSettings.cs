namespace UserService.DataServices.MessageBus;

public class RabbitMQSettings
{
    public required string HostName { get; set; }

    public int Port { get; set; }

    public required string UserName { get; set; }

    public required string Password { get; set; }
}
