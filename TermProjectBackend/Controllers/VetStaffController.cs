using Microsoft.AspNetCore.Mvc;
using System.Net;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;

namespace TermProjectBackend.Controllers
{
    [Route("api/VetStaff")]
    [ApiController]
    public class VetStaffController : Controller
    {
        private readonly IVetStaffService _vetStaffService;

        public  VetStaffController(IVetStaffService vetStaffService)
        {
            _vetStaffService = vetStaffService;
        }

        [HttpPost("CreateStaff")]
        public ActionResult CreateStaff([FromBody] CreateNewStaffDTO dto) {
            
            var staff = _vetStaffService.CreateVetStaff(dto);

            if(staff == null)
            {
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    Status = "Fail",
                    ErrorMessage = "Error adding staff"
                });
            }

            return Ok(new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Status = "Success"
            });
        }

        [HttpDelete("DeleteStaff")]
        public ActionResult DeleteStaff([FromBody] DeleteStaffRequestDTO requestDTO)
        {
            try
            {
                // Assuming userService is an instance of your UserService class
                _vetStaffService.DeleteVetStaff(requestDTO.id);
                return Ok(new { Message = "Staff deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                // Handle the case where the user is not found
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, new { Message = "An error occurred while deleting the staff." });
            }

        }

        [HttpPost("Update")]
        public ActionResult UpdateStaff([FromBody] UpdateVetStaffDTO requestDTO)
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
                _vetStaffService.UpdateVetStaff(requestDTO);
                return Ok(new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Status = "Success"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating information: {ex.Message}");
            }
        }

        [HttpGet("GetAllVetStaff")]
        public ActionResult<List<VetStaff>> GetAllStaff(int page = 1)
        {
            try
            {
                var items = _vetStaffService.GetAllStaff(page, 10);
                var staffInfo = items.Select(v => new GetVetStaff
                {
                    StaffId = v.Id,
                    Name = v.Name,
                    Email = v.Email,
                    Role = v.Role
                }).ToList();
                return Ok(staffInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching items: {ex.Message}");
            }
        }




    }
}
