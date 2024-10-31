using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer.Service
{
    public class RabbitMqConsumerService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private string _queueName = "notification_queue";

        public RabbitMqConsumerService(IConnection connection)
        {
            _connection = connection;
            _channel = _connection.CreateModel();

            // Queue ve Exchange yapılandırması
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: _queueName, exchange: "notification_exchange", routingKey: "notification_routing_key");
        }

        public void StartConsuming()
        {
            var consumerEvent = new EventingBasicConsumer(_channel);

            consumerEvent.Received += (ch, e) =>
            {
                var byteArr = e.Body.ToArray();
                var bodyStr = Encoding.UTF8.GetString(byteArr);

                
                Console.WriteLine($"Received Data: {bodyStr}");

                
                _channel.BasicAck(e.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumerEvent);

            Console.WriteLine($"{_queueName} listening....\n\n\n");
        }

        public void StopConsuming()
        {
           
            if (_channel.IsOpen)
            {
                _channel.Close();
            }
            if (_connection.IsOpen)
            {
                _connection.Close();
            }
        }
    }
}
