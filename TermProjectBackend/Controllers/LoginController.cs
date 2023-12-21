using Microsoft.AspNetCore.Mvc;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;
using System.Net;

namespace se4458_midterm.Controllers
{
    [Route("api/Login")]
    [ApiController]

    public class LoginController : Controller
    {
        private IUserService _userService;
        protected APIResponse _response;
        public LoginController(IUserService userService)
        {
            _userService = userService;
            _response = new();
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var loginResponse = _userService.Login(loginRequestDTO);
            if(loginResponse.APIUser == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Status = "Fail";
                _response.ErrorMessage = "Username or password is invalid";
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }




    }
}
