using GestaoEscalaPermutas.Dominio.Interfaces.Mensageria;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GestaoEscalaPermutas.Dominio.Services.Mensageria
{
    public class RabbitMqMessageBus : IMessageBus
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqMessageBus(string hostName)
        {
            var factory = new ConnectionFactory { HostName = hostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public Task PublishAsync<T>(string queueName, T message)
        {
            // Declara a fila, garantindo que ela seja durável
            _channel.QueueDeclare(
                queue: queueName,
                durable: true, // A fila é durável
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Serializa a mensagem
            var messageBody = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageBody);

            // Configura as propriedades para persistência
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true; // Mensagem persistente

            // Publica a mensagem na fila
            _channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: properties,
                body: body);

            return Task.CompletedTask;
        }

        public void Subscribe<T>(string queueName, Action<T> onMessageReceived)
        {
            _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<T>(body);

                if (message != null)
                {
                    onMessageReceived(message);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
