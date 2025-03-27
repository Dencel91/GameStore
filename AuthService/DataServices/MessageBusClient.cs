using AuthService.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AuthService.DataServices;

public class MessageBusClient : IMessageBusClient
{
    private const string UserRegisteredExchangeName = "UserRegistered";

    private readonly ILogger<MessageBusClient> _logger;
    private IConnection _connection;
    private IChannel _channel;

    public MessageBusClient(IConfiguration configuration, IWebHostEnvironment environment, ILogger<MessageBusClient> logger)
    {
        _logger = logger;

        this.SetConnection(configuration, environment).Wait();
    }

    private async Task SetConnection(IConfiguration configuration, IWebHostEnvironment environment)
    {
        RabbitMQSettings settings;

        if (environment.IsDevelopment())
        {
            settings = new RabbitMQSettings()
            {
                HostName = configuration["RabbitMQ:HostName"],
                Port = int.Parse(configuration["RabbitMQ:Port"]),
                UserName = configuration["RabbitMQ:Username"],
                Password = configuration["RabbitMQ:Password"]
            };
        }
        else
        {
            settings = new RabbitMQSettings()
            {
                HostName = configuration["RabbitMQ:HostName"],
                Port = int.Parse(configuration["RabbitMQ:Port"]),
                UserName = Environment.GetEnvironmentVariable("RabbitMqUserName"),
                Password = Environment.GetEnvironmentVariable("RabbitMqPassword")
            };
        }

        var factory = new ConnectionFactory()
        {
            HostName = settings.HostName,
            Port = settings.Port,
            UserName = settings.UserName,
            Password = settings.Password
        };

        try
        {
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(UserRegisteredExchangeName, ExchangeType.Fanout);

        }
        catch (Exception ex)
        {
            _logger.LogError("Cannot connect to RabbitMQ: {message}", ex.Message);
        }
    }

    public Task PublishUserRegistered(UserRegisteredEvent userRegisteredEvent)
    {
        var message = JsonSerializer.Serialize(userRegisteredEvent);
        return this.SendMessage(message);
    }

    private async Task SendMessage(string message)
    {
        if (!_connection.IsOpen)
        {
            _logger.LogError("RabbitMQ connection is closed. Cannot send a message: {message}", message);
            throw new InvalidOperationException($"RabbitMQ connection is closed. Cannot send a message: {message}");
        }

        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(
            exchange: UserRegisteredExchangeName,
            routingKey: "",
            body: body);

        _logger.LogInformation("Message is sent: {message}", message);
    }

    public void Dispose()
    {
        if (_channel.IsOpen)
        {
            _channel.CloseAsync();
            _connection.CloseAsync();
        }
    }
}
