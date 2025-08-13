using BankingApp.Application.DTOs;
using BankingApp.Application.DTOs.Auth;
using BankingApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthManager _authManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthManager authManager, ILogger<AuthController> logger)
    {
        _authManager = authManager;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _authManager.RegisterAsync(request, "Reader");

            if (!result.Success)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }

            _logger.LogInformation("New user registered: {Email}", request.Email);
            return Ok(new
            {
                Success = true,
                Message = result.Message,
                UserId = result.UserId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }

    [HttpPost("register-admin")]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _authManager.RegisterAsync(request, "Writer");

            if (!result.Success)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }

            _logger.LogInformation("New admin registered: {Email}", request.Email);
            return Ok(new
            {
                Success = true,
                Message = result.Message,
                UserId = result.UserId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin registration");
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var authResponse = await _authManager.LoginAsync(request);

            if (!authResponse.IsAuthenticated)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = authResponse.Message
                });
            }

            _logger.LogInformation("User logged in: {Email}", request.Email);
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }
}
