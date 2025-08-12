using BankingApp.Application.DTOs;
using BankingApp.Application.DTOs.Auth;
using BankingApp.Application.Interfaces;
using BankingApp.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthManager : IAuthManager
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly BankingAuthDbContext _authDbContext;

    public AuthManager(
        UserManager<IdentityUser> userManager,
        IConfiguration configuration,
        BankingAuthDbContext authDbContext)
    {
        _userManager = userManager;
        _configuration = configuration;
        _authDbContext = authDbContext;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            // Get all roles including the seeded Reader/Writer roles
            var roles = await _userManager.GetRolesAsync(user);

            // Get the role names properly normalized
            var allRoles = _authDbContext.Roles.ToList();
            var userRoleNames = allRoles
                .Where(r => roles.Contains(r.Name))
                .Select(r => r.Name)
                .ToList();

            var token = GenerateToken(user, userRoleNames);

            return new AuthResponse
            {
                Token = token,
                RefreshToken = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                RoleList = userRoleNames,
                FullName = user.UserName,
                Email = user.Email,
                UserId = user.Id,
                IsAuthenticated = true,
                Message = "Login successful"
            };
        }

        return new AuthResponse
        {
            IsAuthenticated = false,
            Message = "Invalid email or password"
        };
    }

    public async Task<RegistrationResponse> RegisterAsync(RegisterRequest request, string role = "Reader")
    {
        // Validate role exists
        var roleExists = await _authDbContext.Roles.AnyAsync(r => r.Name == role);
        if (!roleExists)
        {
            return new RegistrationResponse
            {
                Success = false,
                Message = $"Role '{role}' does not exist"
            };
        }

        // Check if user exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new RegistrationResponse
            {
                Success = false,
                Message = "Email already exists"
            };
        }

        // Create new user
        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return new RegistrationResponse
            {
                Success = false,
                Message = "Registration failed",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        // Assign role
        await _userManager.AddToRoleAsync(user, role);

        return new RegistrationResponse
        {
            Success = true,
            Message = "Registration successful",
            UserId = user.Id
        };
    }

    private string GenerateToken(IdentityUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        // Add all roles including Reader/Writer
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["JwtSettings:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}