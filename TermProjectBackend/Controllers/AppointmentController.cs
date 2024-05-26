using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;

namespace TermProjectBackend.Controllers
{
    [Route("api/Appointment")]
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
      //  [Authorize]
        public ActionResult BookAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
        /*    var userId = User.FindFirst("UserId")?.Value;

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
            }*/

            

            try
            {
                var appointment = _appointmentService.BookAppointment(appointmentDTO, appointmentDTO.Id);

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

        [HttpPost("Update")]
        public ActionResult UpdateAppointment([FromBody] ManageAppointmentDTO requestDTO)
        {



            if (requestDTO.Id == 0)
            {
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    Status = "Fail",
                    ErrorMessage = "Error update item. check id"
                });
            }

            

            try
            {
                _appointmentService.UpdateAppointment(requestDTO);
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
                return StatusCode(500, new { Message = "An error occurred while updating appointment." });
            }
        }

        [HttpDelete("Delete")]
        public ActionResult DeleteAppointment(int id)
        {
            try
            {
                // Assuming userService is an instance of your UserService class
                _appointmentService.RemoveAppointment(id);
                return Ok(new { Message = "Appointment deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                // Handle the case where the user is not found
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, new { Message = "An error occurred while deleting the appointment." });
            }
        }

        [HttpGet("GetAllAppointments")]
        public ActionResult<List<Appointment>> GetAllAppointments(int page = 1)
        {
            try
            {
                var items = _appointmentService.GetAppointmentsPerPage(page, 10);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching items: {ex.Message}");
            }
        }

        [HttpGet("GetUserAppointments")]
        public ActionResult<List<Appointment>> GetUserAppointments(int page = 1, int userId = 0)
        {
            try
            {
                var appointments = _appointmentService.GetUserAppointments(page, 10, userId);
                var userAppointments = appointments.Select(a => new AppointmentDTO
                {
                    Id = a.AppointmentId,
                    AppointmentDateTime = a.AppointmentDateTime,
                    PetName = a.PetName,
                    Reasons = a.Reasons
                }).ToList();
                return Ok(userAppointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching items: {ex.Message}");
            }
        }
    }
}
