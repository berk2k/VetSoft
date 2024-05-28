using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public interface INotificationService
    {
        public void Notification(NotificationRequestDTO notificationRequest);

        public List<Notification> GetUserNotification(int page, int pageSize, int userId);

        public List<Notification> GetUserNotificationWOPagination(int userId);
        public string getName(int id);

        public void SendMessageToVet(VetMessageDTO vetMessageDTO);

        public List<VeterinarianMessages> GetVeterinarianMessages(int userId);
    }
}
