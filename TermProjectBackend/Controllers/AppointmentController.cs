using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TermProjectBackend.Models;
using TermProjectBackend.Source.Svc;

namespace TermProjectBackend.Controllers
{
    [Route("api/AddPet")]
    [ApiController]
    public class AppointmentController : Controller
    {
        private readonly APIResponse _response;
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _response = new APIResponse();
            _appointmentService = appointmentService;
        }

        [HttpPost("BookAppointment")]
        [Authorize]
        public ActionResult BookAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new APIResponse
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    IsSuccess = false,
                    Status = "Fail",
                    ErrorMessage = "User not authenticated"
                });
            }

            if (!int.TryParse(userId, out int parsedUserId))
            {
                // Handle conversion failure
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    Status = "Fail",
                    ErrorMessage = "Invalid user ID format"
                });
            }

            var appointment = _appointmentService.BookAppointment(appointmentDTO, parsedUserId);

            if (appointment == null)
            {
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    Status = "Fail",
                    ErrorMessage = "Error booking appointment"
                });
            }

            return Ok(new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Status = "Success"
            });
        }
    }
}
