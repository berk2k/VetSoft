
using Newtonsoft.Json;

using RabbitMQ.Client;

using System.Text;


namespace TermProjectBackend.Source.Svc
{
    public class RabbitMqService
    {
        private readonly IConnection _connection;
        private IModel _channel;

        private const string exchangeName = "notification_exchange";
        private const string queueName = "notification_queue";
        private const string routingKey = "notification_routing_key";

        public RabbitMqService(IConnection connection)
        {
            _connection = connection;
            _channel = CreateOrGetChannel();
            InitializeQueueAndExchange();
        }

        private void InitializeQueueAndExchange()
        {



            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

            // declare queue (if not exists)
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            // bind queue with exchange
            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);
        }


        public void DeclareQueue(string queueName)
        {
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }


        public void DeclareExchange(string exchangeName, string exchangeType)
        {
            _channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType);
        }


        public void BindQueue(string queueName, string exchangeName, string routingKey)
        {
            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);
        }


        public void PublishMessage<T>(T message)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
        }


        public string GetExchangeName()
        {
            return exchangeName;
        }


        public string GetRoutingKey()
        {
            return routingKey;
        }


        private IModel CreateOrGetChannel()
        {
            return _channel ?? (_channel = _connection.CreateModel());
        }


        public void CloseConnection()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}


