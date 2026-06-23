using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityService.Services;
using Shared.Models;
using System.Threading.Tasks;

namespace IdentityService.Controllers
{
    [Route("api/v{version:apiVersion}/auth")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _authService.LoginAsync(request);
            if (response == null) return Unauthorized(new { success = false, message = "Invalid username or password" });

            return Ok(new { success = true, data = response });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _authService.RefreshTokenAsync(request);
            if (response == null) return Unauthorized(new { success = false, message = "Invalid or expired token" });

            return Ok(new { success = true, data = response });
        }

        [Authorize]
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.RevokeTokenAsync(request.Token);
            if (!result) return NotFound(new { success = false, message = "Token not found or already revoked" });

            return Ok(new { success = true, message = "Token successfully revoked" });
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnly()
        {
            return Ok(new { success = true, message = "Welcome Admin!" });
        }
    }
}
