using OrderService.DataServices;
using ProductService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ProductService.DataServices;

public class MessageBusSubscriber : BackgroundService
{
    private const string PurchaseCompletedExchangeName = "PurchaseCompleted";

    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection;
    private IChannel _channel;
    private string _queueName;

    public MessageBusSubscriber(IConfiguration configuration, IWebHostEnvironment environment, IEventProcessor eventProcessor)
    {
        _eventProcessor = eventProcessor;

        this.InitializeMessageBus(configuration, environment).Wait();
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
            await _channel.ExchangeDeclareAsync(PurchaseCompletedExchangeName, ExchangeType.Fanout);
            _queueName = (await _channel.QueueDeclareAsync(exclusive: false)).QueueName;
            await _channel.QueueBindAsync(_queueName, PurchaseCompletedExchangeName, "");

            Console.WriteLine("Connected to Message Bus");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cannot connect to Message Bus: {ex.Message}");
        }
    }

    public void Dispose()
    {
        Console.WriteLine("RabbitMQ disposed");

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
            Console.WriteLine("Event received");
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            await _eventProcessor.ProcessEvent(content);
            
        };

        await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);
    }
}
