using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public interface INotificationService
    {
        public void Notification(NotificationRequestDTO notificationRequest);

        public string getName(int id);
    }
}
