using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using UserService.DataServices.MessageBus.EventProcessing;

namespace UserService.DataServices.MessageBus;

public class MessageBusSubscriber : BackgroundService
{
    private const string UserRegisteredExchangeName = "UserRegistered";
    private const string PurchaseCompletedExchangeName = "PurchaseCompleted";
    private const string UserRegisteredQueueName = "UserRegisteredQueue";
    private const string PurchaseCompletedQueueName = "PurchaseCompletedQueue";


    private readonly IEventProcessor _eventProcessor;
    private readonly ILogger<MessageBusSubscriber> _logger;
    private IConnection _connection;
    private IChannel _channel;

    public MessageBusSubscriber(
        IConfiguration configuration,
        IWebHostEnvironment environment,
        IEventProcessor eventProcessor,
        ILogger<MessageBusSubscriber> logger)
    {
        _eventProcessor = eventProcessor;
        _logger = logger;

        InitializeMessageBus(configuration, environment).Wait();
    }

    private async Task InitializeMessageBus(IConfiguration configuration, IWebHostEnvironment environment)
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
            await _channel.QueueDeclareAsync(exclusive: false, queue: UserRegisteredQueueName, durable: true, autoDelete: false);
            await _channel.QueueBindAsync(UserRegisteredQueueName, UserRegisteredExchangeName, "");

            await _channel.ExchangeDeclareAsync(PurchaseCompletedExchangeName, ExchangeType.Fanout);
            await _channel.QueueDeclareAsync(exclusive: false, queue: PurchaseCompletedQueueName, durable: true, autoDelete: false);
            await _channel.QueueBindAsync(PurchaseCompletedQueueName, PurchaseCompletedExchangeName, "");
        }
        catch (Exception ex)
        {
            _logger.LogError("Cannot connect to Message Bus: {message}", ex.Message);
        }
    }

    public void Dispose()
    {
        if (_channel.IsOpen)
        {
            _channel.CloseAsync();
            _connection.CloseAsync();
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            _logger.LogInformation("Event received: {content}", content);
            await _eventProcessor.ProcessEvent(content);
        };

        try
        {
            await _channel.BasicConsumeAsync(queue: UserRegisteredQueueName, autoAck: true, consumer: consumer);
            await _channel.BasicConsumeAsync(queue: PurchaseCompletedQueueName, autoAck: true, consumer: consumer);
        }
        catch (Exception ex)
        {
            _logger.LogError("Cannot consume queue: {message}", ex.Message);
        }
    }
}
