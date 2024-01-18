using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;

namespace TermProjectBackend.Controllers
{
    [Route("api/UserProfile")]
    [ApiController]
    public class UserProfileController : Controller
    {
        private IUserService _userService;
        protected APIResponse _response;

        public UserProfileController(IUserService userService)
        {
            _userService = userService;
            _response = new APIResponse();
        }

        [HttpGet]
    //    [Authorize]
        public ActionResult<UserProfileDTO> GetUserInfo(int id)
        {

        /*    var userId = User.FindFirst("UserId")?.Value;

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

            // Retrieve user information based on the ID
            try
            {
                User user = _userService.GetUserInformationById(id);

                // Map the User entity to UserProfileDTO
                var userProfileDTO = new UserProfileDTO
                {
                    Id = id,
                    UserName = user.UserName,
                    Name = user.Name
                };

                // Return the UserProfileDTO
                return userProfileDTO;
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an appropriate response
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    Status = "Fail",
                    ErrorMessage = $"Error retrieving user information: {ex.Message}"
                });
            }
        }

        [HttpPost("Delete")]
        public ActionResult DeleteCustomer([FromBody] DeletePetRequestDTO deletePetRequestDTO)
        {
            try
            {
                // Assuming userService is an instance of your UserService class
                _userService.DeleteAccount(deletePetRequestDTO.id);
                return Ok(new { Message = "Customer deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                // Handle the case where the user is not found
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, new { Message = "An error occurred while deleting the customer." });
            }
        }

    }
}
