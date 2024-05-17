using Microsoft.AspNetCore.Mvc;
using System.Net;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;

namespace TermProjectBackend.Controllers
{
    [Route("api/Notification")]
    [ApiController]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;

        }

        [HttpPost("SendNotification")]
        // [Authorize]
        public ActionResult Send([FromBody] NotificationRequestDTO notificationRequest)
        {
            _notificationService.Notification(notificationRequest);

            return Ok(new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Status = "Success"
            });
        }

        [HttpPost("SendMessageFromUserToVet")]
        // [Authorize]
        public ActionResult SendMessageToVet([FromBody] VetMessageDTO notificationRequest)
        {
            _notificationService.SendMessageToVet(notificationRequest);

            return Ok(new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Status = "Success"
            });
        }

        [HttpGet("GetNotificationHistoryForUser")]
        public ActionResult<List<Notification>> GetNotificationsForUser(int page = 1, int userId = 0)
        {
            try
            {
                var notifications = _notificationService.GetUserNotification(page, 10, userId);
                var userNot = notifications.Select(n => new NotificationRequestDTO
                {
                    userId = n.userId,
                    message = n.message,
                    SentAt = n.SentAt
                }).ToList();
                return Ok(userNot);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching items: {ex.Message}");
            }
        }
    }
}
