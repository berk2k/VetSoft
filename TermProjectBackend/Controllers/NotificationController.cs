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
            
            try
            {
                _notificationService.Notification(notificationRequest);
                return Ok(new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Status = "Success"
                });
            }
            catch (InvalidOperationException ex)
            {
                // Handle the case where the user is not found
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, new { Message = "An error occurred while sending message." });
            }
            
        }

        [HttpPost("SendMessageFromUserToVet")]
        // [Authorize]
        public ActionResult SendMessageToVet([FromBody] VetMessageDTO notificationRequest)
        {
            

            try
            {
                _notificationService.SendMessageToVet(notificationRequest);
                return Ok(new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Status = "Success"
                });
            }
            catch (InvalidOperationException ex)
            {
                // Handle the case where the user is not found
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, new { Message = "An error occurred while sending message." });
            }
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
            catch (InvalidOperationException ex)
            {
                // Handle the case where the user is not found
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, new { Message = "An error occurred while adding the record." });
            }
        }

        [HttpGet("GetNotificationHistoryForUserWOPagination")]
        public ActionResult<List<Notification>> GetNotificationsForUserWOPagination(int userId = 0)
        {
            try
            {
                var notifications = _notificationService.GetUserNotificationWOPagination(userId);
                var userNot = notifications.Select(n => new NotificationRequestDTO
                {
                    userId = n.userId,
                    message = n.message,
                    SentAt = n.SentAt
                }).ToList();
                return Ok(userNot);
            }
            catch (InvalidOperationException ex)
            {
                // Handle the case where the user is not found
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, new { Message = "An error occurred while adding the record." });
            }
        }

        [HttpGet("GetNotificationHistoryForVet")]
        public ActionResult<List<Notification>> GetNotificationsForVet(int userId = 0)
        {
            try
            {
                var notifications = _notificationService.GetVeterinarianMessages(userId);
                var userNot = notifications.Select(n => new VetMessageDTO
                {
                    userId = n.UserId,
                    messageTitle = n.MessageTitle,
                    messageText = n.MessageText,
                    SentAt = n.SentAt
                }).ToList();
                return Ok(userNot);
            }
            catch (InvalidOperationException ex)
            {
                // Handle the case where the user is not found
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, new { Message = "An error occurred while adding the record." });
            }
        }
    }
}
