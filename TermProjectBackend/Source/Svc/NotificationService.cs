using TermProjectBackend.Context;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Models;
using System.Text;
using RabbitMQ.Client;

namespace TermProjectBackend.Source.Svc
{
    public class NotificationService : INotificationService
    {
        private readonly VetDbContext _vetDb;
        private readonly ConnectionFactory _connectionFactory;
        private const string QueueName = "notification_queue";

        //private readonly RabbitMqService _rabbitMqService;
        public NotificationService(VetDbContext vetDb/*,RabbitMqService rabbitMqService*/)
        {

            _vetDb = vetDb;
            //_rabbitMqService = rabbitMqService;

            //_connectionFactory = new ConnectionFactory
            //{
            //    HostName = "localhost", // RabbitMQ sunucu adresi
            //    Port = 5672, // RabbitMQ varsayılan bağlantı noktası
            //    UserName = "guest", // RabbitMQ kullanıcı adı
            //    Password = "guest" // RabbitMQ şifre
            //};
            
        }
        public string getName(int userId)
        {
            var user = _vetDb.Users.Find(userId);

            if (user != null)
            {
                return user.UserName;
            }
            else
            {
                // Handle case where user is not found
                return null; // Or throw an exception, return a default value, etc.
                return null; // Or throw an exception, return a default value, etc.
            }
        }


        public void Notification(NotificationRequestDTO notificationRequest)
        {
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"); // Türkiye'nin standart saat dilimi
            DateTime trTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tzi);

            var user = _vetDb.Users.Find(notificationRequest.userId);

            if (user == null)
            {
                // Handle the case where the user is not found
                throw new InvalidOperationException($"User with ID {notificationRequest.userId} not found.");
            }
            // Create a new Notification instance
            Notification newNotification = new Notification
            {
                message = notificationRequest.message,
                userId = notificationRequest.userId,
                userName = getName(notificationRequest.userId),
                SentAt = trTime
            };

            // Add the new notification to the Notifications DbSet
            _vetDb.Notification.Add(newNotification);

            // Save changes to the database
            _vetDb.SaveChanges();

            //send message to rabbitmq
            //SendMessageToRabbitMQ(newNotification);
        }


        //private void SendMessageToRabbitMQ(Notification newNotification)
        //{
            
        //    using (var connection = _connectionFactory.CreateConnection())
        //    using (var channel = connection.CreateModel())
        //    {
                
        //        channel.QueueDeclare(queue: QueueName,
        //                             durable: false,
        //                             exclusive: false,
        //                             autoDelete: false,
        //                             arguments: null);

        //        channel.ExchangeDeclare("direct_exchange", ExchangeType.Fanout, true);

                
        //        string message = Newtonsoft.Json.JsonConvert.SerializeObject(newNotification);
        //        var body = Encoding.UTF8.GetBytes(message);

               



                
        //        channel.BasicPublish(exchange: "direct_exchange",
        //                             routingKey: QueueName,
        //                             basicProperties: null,
        //                             body: body);
        //        channel.Close();
        //        connection.Close();
        //    }
        //}



        public List<Notification> GetUserNotification(int page, int pageSize, int userId)
        {
            return _vetDb.Notification
                .Where(n => n.userId == userId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public void SendMessageToVet(VetMessageDTO vetMessageDTO)
        {
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"); // Türkiye'nin standart saat dilimi
            DateTime trTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tzi);

            var user = _vetDb.Users.Find(vetMessageDTO.userId);

            if (user == null)
            {
                // Handle the case where the user is not found
                throw new InvalidOperationException($"User with ID {vetMessageDTO.userId} not found.");
            }

            VeterinarianMessages newNotification = new VeterinarianMessages
            {
                MessageText = vetMessageDTO.messageText,
                MessageTitle = vetMessageDTO.messageTitle,
                UserId = vetMessageDTO.userId,
                UserName = getName(vetMessageDTO.userId),
                SentAt = trTime
            };

            // Add the new notification to the Notifications DbSet
            _vetDb.VeterinarianMessages.Add(newNotification);

            // Save changes to the database
            _vetDb.SaveChanges();

            //string message = Newtonsoft.Json.JsonConvert.SerializeObject(newNotification);
            //_rabbitMqService.SendMessageToRabbitMQ(QueueName, message);

        }

        public List<Notification> GetUserNotificationWOPagination(int userId)
        {
            return _vetDb.Notification
            .Where(n => n.userId == userId).ToList();
        }

        public List<VeterinarianMessages> GetVeterinarianMessages(int userId)
        {
            return _vetDb.VeterinarianMessages
            .Where(n => n.UserId == userId).ToList();
        }
    }
}
