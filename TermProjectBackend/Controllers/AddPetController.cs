using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;

namespace TermProjectBackend.Controllers
{
    [Route("api/AddPet")]
    [ApiController]
    public class AddPetController : ControllerBase
    {
        private readonly IPetService _petService;

        public AddPetController(IPetService petService)
        {
            _petService = petService;
        }

        [HttpPost("Add")]
        [Authorize]
        public ActionResult AddPet([FromBody] AddPetRequestDTO addPetRequestDTO)
        {
            // Check if pet already exists
            bool isPetUnique = _petService.IsPetUnique(addPetRequestDTO.Name);
            if (!isPetUnique)
            {
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    Status = "Fail",
                    ErrorMessage = "Pet already exists"
                });
            }

            // Get user ID from the authenticated user claims
            var userId = User.FindFirst("UserId")?.Value;  // Use "UserId" as the claim type
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

            // Convert userId to int if necessary
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

            // Add pet with the obtained user ID
            var pet = _petService.AddPet(addPetRequestDTO, parsedUserId);

            if (pet == null)
            {
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    Status = "Fail",
                    ErrorMessage = "Error adding pet"
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
