using BankingApp.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BankingApp.Application.Interfaces;

namespace BankingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public LoginController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authResponse = await _authManager.LoginAsync(request);

            if (!authResponse.IsAuthenticated)
                return Unauthorized(new
                {
                    Success = false,
                    Message = authResponse.Message
                });

            return Ok(new
            {
                Success = true,
                Message = authResponse.Message,
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken,
                ExpiresAt = authResponse.ExpiresAt,
                Roles = authResponse.RoleList,
                FullName = authResponse.FullName,
                Email = authResponse.Email,
                UserId = authResponse.UserId
            });
        }
    }
}
