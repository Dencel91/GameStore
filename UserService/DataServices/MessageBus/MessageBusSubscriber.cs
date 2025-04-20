using GameStore.Common.Helpers;
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
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"] ?? throw new InvalidOperationException("Config error: RabbitMQ:HostName is not set "),
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? throw new InvalidOperationException("Config error: RabbitMQ:Port is not set ")),
                UserName = ConfigHelper.GetSecret(environment, configuration, "RabbitMq-UserName"),
                Password = ConfigHelper.GetSecret(environment, configuration, "RabbitMq-Password")
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await BindQueue(UserRegisteredExchangeName, UserRegisteredQueueName);
            await BindQueue(PurchaseCompletedExchangeName, PurchaseCompletedQueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError("Cannot connect to Message Bus: {message}", ex.Message);
            throw;
        }
    }

    private async Task BindQueue(string exchangeName, string queueName)
    {
        await _channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout);
        await _channel.QueueDeclareAsync(exclusive: false, queue: queueName, durable: true, autoDelete: false);
        await _channel.QueueBindAsync(queueName, exchangeName, "");
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
