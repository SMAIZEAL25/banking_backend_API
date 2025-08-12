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
        try
        {
            var result = await _authManager.RegisterAsync(request, "Reader");

            if (!result.Success)
                return BadRequest(result);

            _logger.LogInformation("New user registered: {Email}", request.Email);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpPost("register-admin")]
    [Authorize(Roles = "Writer")] 
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authManager.RegisterAsync(request, "Writer");

            if (!result.Success)
                return BadRequest(result);

            _logger.LogInformation("New admin registered: {Email}", request.Email);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin registration");
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var authResponse = await _authManager.LoginAsync(request);

            if (!authResponse.IsAuthenticated)
                return Unauthorized(new { authResponse.Message });

            _logger.LogInformation("User logged in: {Email}", request.Email);
            return Ok(authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }
}