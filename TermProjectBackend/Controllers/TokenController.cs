using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TermProjectBackend.Extensions;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc.Interfaces;

namespace TermProjectBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : BaseController
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService; 

        public TokenController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            var result = await _tokenService.RefreshAccessTokenAsync(request.RefreshToken);

            if (!result.Success)
            {
                return Unauthorized(new { message = result.Error });
            }

            return Ok(new
            {
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                expiresAt = result.RefreshTokenExpiry
            });
        }

        [Authorize]
        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
        {
            
            var userId = User.GetUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _tokenService.RevokeRefreshTokenAsync((int)userId, request.RefreshToken);

            return Ok(new { message = "Token revoked successfully" });
        }
    }
}
