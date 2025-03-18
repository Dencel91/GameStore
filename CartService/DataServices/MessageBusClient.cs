using CartService.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CartService.DataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private const string PurchaseCompletedExchangeName = "PurchaseCompleted";
        private IConnection _connection;
        private IChannel _channel;

        public MessageBusClient(IConfiguration configuration, IWebHostEnvironment environment)
        {
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
                await _channel.ExchangeDeclareAsync(PurchaseCompletedExchangeName, ExchangeType.Fanout);
                Console.WriteLine("Connected to RabbitMQ");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot connect to RabbitMQ: {ex.Message}");
            }
        }

        public Task PublishPurchaseCompleted(PurchaseCompletedEvent purchaseCompletedEvent)
        {
            var message = JsonSerializer.Serialize(purchaseCompletedEvent);
            return this.SendMessage(message);
        }

        private async Task SendMessage(string message)
        {
            if (!_connection.IsOpen)
            {
                Console.WriteLine("RabbitMQ connection is closed");
                return;
            }

            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: PurchaseCompletedExchangeName,
                routingKey: "",
                body: body);

            Console.WriteLine($"Message is sent: {message}");
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
    }
}
