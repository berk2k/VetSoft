using Microsoft.AspNetCore.Mvc;
using System.Net;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Models;
using TermProjectBackend.Source.Svc;
using Microsoft.Data.SqlClient;

namespace TermProjectBackend.Controllers
{
    [Route("api/VaccinationRecord")]
    [ApiController]
    public class VaccinationRecordController : Controller
    {
        private readonly IVaccinationService _vaccinationService;

        public VaccinationRecordController(IVaccinationService vaccinationService)
        {
           _vaccinationService = vaccinationService;
        }


        [HttpPost("AddVaccinationRecord")]
         

        public ActionResult AddRecord([FromBody] AddVaccinationRecordRequestDTO requestDTO)
        {
            try
            {
                var record = _vaccinationService.AddVaccinationRecord(requestDTO);
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
            //var record = _vaccinationService.AddVaccinationRecord(requestDTO);

            //if (record == null)
            //{
            //    return BadRequest(new APIResponse
            //    {
            //        StatusCode = HttpStatusCode.BadRequest,
            //        IsSuccess = false,
            //        Status = "Fail",
            //        ErrorMessage = "Error adding record"
            //    });
            //}

            //return Ok(new APIResponse
            //{
            //    StatusCode = HttpStatusCode.OK,
            //    IsSuccess = true,
            //    Status = "Success"
            //});
            //try
            //{
            //    var record = _vaccinationService.AddVaccinationRecord(requestDTO);

            //    if (record == null)
            //    {
            //        return BadRequest(new APIResponse
            //        {
            //            StatusCode = HttpStatusCode.BadRequest,
            //            IsSuccess = false,
            //            Status = "Fail",
            //            ErrorMessage = "Error adding record"
            //        });
            //    }

            //    return Ok(new APIResponse
            //    {
            //        StatusCode = HttpStatusCode.OK,
            //        IsSuccess = true,
            //        Status = "Success"
            //    });
            //}
            //catch (SqlException ex)
            //{
            //// Check if the exception is related to foreign key constraint violation
            //    if (ex.Number == 547) // Error number for foreign key constraint violation
            //    {
            //        return BadRequest(new APIResponse
            //        {
            //            StatusCode = HttpStatusCode.BadRequest,
            //            IsSuccess = false,
            //            Status = "Fail",
            //            ErrorMessage = "The specified user ID or pet ID does not exist."
            //        });
            //    }
            //    else
            //    {
            //        // Handle other SQL exceptions
            //        return StatusCode(500, new APIResponse
            //        {
            //            StatusCode = HttpStatusCode.InternalServerError,
            //            IsSuccess = false,
            //            Status = "Error",
            //            ErrorMessage = "An error occurred while processing the request."
            //        });
            //    }
            //}
            //catch (Exception ex)
            //{
            //// Handle other exceptions
            //    return StatusCode(500, new APIResponse
            //    {
            //        StatusCode = HttpStatusCode.InternalServerError,
            //        IsSuccess = false,
            //        Status = "Error",
            //        ErrorMessage = "An error occurred while processing the request."
            //    });
            //}
        }


        [HttpDelete("Delete")]
        public ActionResult DeleteRecord(int id)
        {
            try
            {
               
                _vaccinationService.RemoveVaccinationReport(id);
                return Ok(new { Message = "Record deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                // Handle the case where the user is not found
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, new { Message = "An error occurred while deleting the record." });
            }
        }

        [HttpGet("GetAllVaccinationHistoryForUser")]
        public ActionResult<List<GetVaccinationReportDTO>> GetAllVaccinationHistoryForUser(int id)
        {
            try
            {
                var vaccinationHistory = _vaccinationService.GetAllVaccinationHistoryForUser(id);
                return Ok(vaccinationHistory);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return StatusCode(500, new { Message = "An error occurred while retrieving vaccination history." });
            }
        }
    }
}
