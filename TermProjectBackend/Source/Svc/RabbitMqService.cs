using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class RabbitMqService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

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

    }

    public List<string> GetMessages(string queueName)
    {
        var messages = new List<string>();
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            messages.Add(message);
        };

        _channel.BasicConsume(queue: queueName,
                             autoAck: true,
                             consumer: consumer);

        // Biraz bekleyin ki mesajlar işlenebilsin
        System.Threading.Thread.Sleep(1000);

        return messages;
    }
}
