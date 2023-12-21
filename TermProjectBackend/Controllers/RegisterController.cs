using Microsoft.AspNetCore.Mvc;
using System.Net;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;

namespace TermProjectBackend.Controllers
{
    [Route("api/Register")]
    [ApiController]
    public class RegisterController : Controller
    {
        private IUserService _userService;
        protected APIResponse _response;
        public RegisterController(IUserService userService)
        {
            _userService = userService;
            _response = new();
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterationRequestDTO registerationRequestDTO)
        {
            bool isUserUnique = _userService.IsUserUnique(registerationRequestDTO.UserName);

            if (!isUserUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Status = "Fail";
                _response.ErrorMessage = "User already exists";
                return BadRequest(_response);
            }

            var user = _userService.Register(registerationRequestDTO);

            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Status = "Fail";
                _response.ErrorMessage = "error";
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);

        }
    }
}
