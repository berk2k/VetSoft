using Microsoft.AspNetCore.Mvc;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;
using System.Net;
using TermProjectBackend.Models;

namespace TermProjectBackend.Controllers
{
    [Route("api/Login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IVetStaffService _vetStaffService;
        private readonly APIResponse _response;

        public LoginController(IUserService userService, IVetStaffService vetStaffService)
        {
            _userService = userService;
            _response = new APIResponse();
            _vetStaffService = vetStaffService;
        }

        [HttpPost("login")]
        public ActionResult<APIResponse> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var loginResponse = _userService.Login(loginRequestDTO);

            if (loginResponse.APIUser == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Status = "Fail";
                _response.ErrorMessage = "Invalid username or password.";
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("LoginForStaff")]
        public ActionResult<APIResponse> LoginForWeb([FromBody] LoginRequestVetStaffDTO loginRequestDTO)
        {
            var loginResponse = _vetStaffService.Login(loginRequestDTO);

            if (loginResponse.APIUser == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Status = "Fail";
                _response.ErrorMessage = "Invalid email or password.";
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }
    }
}
