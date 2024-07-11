using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using TermProjectBackend.Models;

public class RabbitMqService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string ExchangeName = "direct_exchange";

    public RabbitMqService()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",  // Kullanıcı adı
            Password = "guest",  // Parola
            Port = 5672          // Port
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout, durable: true);

    }

    public List<string> GetMessages(string queueName)
    {
        var messages = new List<string>();
        var consumer = new EventingBasicConsumer(_channel);
        

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);
            messages.Add(message);
            

            // Signal that we have received a message
            
        };

        _channel.BasicConsume(queue: queueName,
                             autoAck: true,
                             consumer: consumer);


        return messages;
    }

    public void SendMessageToRabbitMQ(string queueName, string message)
    {
        _channel.QueueDeclare(queue: queueName,
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: ExchangeName,
                              routingKey: queueName,
                              basicProperties: null,
                              body: body);
    }
}
