using TermProjectBackend.Context;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Models;
using System.Text;
using RabbitMQ.Client;
using TermProjectBackend.Source.Svc.Interfaces;
using Microsoft.AspNetCore.SignalR;
using TermProjectBackend.Hubs;

namespace TermProjectBackend.Source.Svc
{
    public class NotificationService : INotificationService
    {
        private readonly VetDbContext _vetDb;
        private readonly IHubContext<NotificationHub> _hubContext;


        public NotificationService(VetDbContext vetDb, IHubContext<NotificationHub> hubContext)
        {
            _vetDb = vetDb;
            _hubContext = hubContext;
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
               
                return "user not found";
                
            }
        }


        public void Notification(NotificationRequestDTO notificationRequest)
        {
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            DateTime trTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tzi);

            var user = _vetDb.Users.Find(notificationRequest.userId);

            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {notificationRequest.userId} not found.");
            }

            Notification newNotification = new Notification
            {
                message = notificationRequest.message,
                userId = notificationRequest.userId,
                userName = getName(notificationRequest.userId),
                SentAt = trTime
            };

            _vetDb.Notification.Add(newNotification);
            _vetDb.SaveChanges();

            
            _hubContext.Clients.User(notificationRequest.userId.ToString())
                .SendAsync("ReceiveNotification", new
                {
                    newNotification.message,
                    sentAt = newNotification.SentAt,
                    newNotification.userName
                });
        }


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
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            DateTime trTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tzi);

            var user = _vetDb.Users.Find(vetMessageDTO.userId);

            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {vetMessageDTO.userId} not found.");
            }

            VeterinarianMessages newMessage = new VeterinarianMessages
            {
                MessageText = vetMessageDTO.messageText,
                MessageTitle = vetMessageDTO.messageTitle,
                UserId = vetMessageDTO.userId,
                UserName = getName(vetMessageDTO.userId),
                SentAt = trTime
            };

            _vetDb.VeterinarianMessages.Add(newMessage);
            _vetDb.SaveChanges();

            
            _hubContext.Clients.User(vetMessageDTO.userId.ToString())
                .SendAsync("ReceiveVetMessage", new
                {
                    messageTitle = newMessage.MessageTitle,
                    messageText = newMessage.MessageText,
                    userName = newMessage.UserName,
                    sentAt = newMessage.SentAt
                });
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
