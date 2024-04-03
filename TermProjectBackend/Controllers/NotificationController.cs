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
    }
}
