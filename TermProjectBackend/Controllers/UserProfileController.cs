using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TermProjectBackend.Extensions;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc.Interfaces;

namespace TermProjectBackend.Controllers
{
    [Route("api/UserProfile")]
    [ApiController]
    public class UserProfileController : BaseController
    {
        private IUserService _userService;
        protected APIResponse _response;

        public UserProfileController(IUserService userService)
        {
            _userService = userService;
            _response = new APIResponse();
        }

        [HttpGet("GetUserIdByUserName")]
        public ActionResult GetUserIdByName(string userName) {
            try
            {
                // Assuming userService is an instance of UserService injected into the controller
                int userId = _userService.getUserIdByName(userName);
                return Ok(userId);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return StatusCode(500, "Internal server error");
            }


        }

        [HttpGet("GetAllUsers")]
        public ActionResult<List<User>> GetAllUsers()
        {
            try
            {
                // Fetch all users from the data source
                var users = _userService.GetAllUserNames();

                // Return the list of users
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("GetUserInfo")]
        [Authorize]
        public ActionResult<UserProfileDTO> GetUserInfo()
        {
            try
            {
                var userId = User.GetUserId();
                if (userId == null)
                    return Unauthorized();
                User user = _userService.GetUserInformationById((int)userId);

                // Map the User entity to UserProfileDTO
                var userProfileDTO = new UserProfileDTO
                {
                    Id = (int)userId,
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
