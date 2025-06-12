using Microsoft.AspNetCore.Mvc;
using TermProjectBackend.Models.Dto;
using System.Net;
using TermProjectBackend.Models;
using TermProjectBackend.Source.Svc.Interfaces;

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
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var loginResponse = await _userService.Login(loginRequestDTO);

            if (string.IsNullOrEmpty(loginResponse.Token))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            //Response.Cookies.Append("refreshToken", loginResponse.RefreshToken, new CookieOptions
            //{
            //    HttpOnly = true,
            //    Expires = loginResponse.RefreshTokenExpiryDate,
            //    Secure = true, // Use in production with HTTPS
            //    SameSite = SameSiteMode.Strict
            //});

            return Ok(new
            {
                token = loginResponse.Token,
                refreshToken = loginResponse.RefreshToken, // Can also be omitted if using cookies
                expiresAt = loginResponse.RefreshTokenExpiryDate,
                user = loginResponse.APIUser
            });
        }

        [HttpPost("Staff")]
        public async Task<IActionResult> LoginForWeb([FromBody] LoginRequestVetStaffDTO loginRequestDTO)
        {
            var loginResponse = await _vetStaffService.Login(loginRequestDTO);
            if (string.IsNullOrEmpty(loginResponse.Token))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            return Ok(new
            {
                token = loginResponse.Token,
                //refreshToken = loginResponse.RefreshToken,
                //expiresAt = loginResponse.RefreshTokenExpiryDate,
                user = loginResponse.APIUser
            });
        }
    }
}
