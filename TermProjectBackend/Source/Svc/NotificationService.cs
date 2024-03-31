using TermProjectBackend.Context;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Models;

namespace TermProjectBackend.Source.Svc
{
    public class NotificationService : INotificationService
    {
        private readonly VetDbContext _vetDb;
        public NotificationService(VetDbContext vetDb) {

            _vetDb = vetDb;

        }
        public string getName(int userId)
        {
            var user = _vetDb.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return user.UserName;
            }
            else
            {
                // Handle case where user is not found
                return null; // Or throw an exception, return a default value, etc.
            }
        }

        public void Notification(NotificationRequestDTO notificationRequest)
        {
            
            // Create a new Notification instance
            Notification newNotification = new Notification
            {
                message = notificationRequest.message,
                userId = notificationRequest.userId,
                userName = getName(notificationRequest.userId),
            };

            // Add the new notification to the Notifications DbSet
            _vetDb.Notification.Add(newNotification);

            // Save changes to the database
            _vetDb.SaveChanges();
        }
    }
}
