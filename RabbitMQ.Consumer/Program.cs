using RabbitMQ.Client;
using RabbitMQ.Consumer.Service;
using System;

namespace RabbitMQ.Consumer
{
    public class Program
    {
        private static string connectionString = "amqp://guest:guest@localhost:5672";

        static void Main(string[] args)
        {
            var queueName = args.Length > 0 ? args[0] : "notification_queue";

            using var connection = GetConnection();
            var consumerService = new RabbitMqConsumerService(connection);

            // Start consuming messages
            consumerService.StartConsuming();

            Console.WriteLine($"{queueName} listening....\nPress Enter to exit.");

            // Wait for user input to stop consuming
            Console.ReadLine();

            // Cleanly stop consuming
            consumerService.StopConsuming();
        }

        private static IConnection GetConnection()
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(connectionString, UriKind.RelativeOrAbsolute)
            };

            return factory.CreateConnection();
        }
    }
}