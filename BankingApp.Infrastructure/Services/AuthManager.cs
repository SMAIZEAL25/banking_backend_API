using BankingApp.Application.DTOs;
using BankingApp.Application.DTOs.Auth;
using BankingApp.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthManager : IAuthManager
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthManager(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Email);

        if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateToken(user, roles);

            return new AuthResponse
            {
                Token = token,
                RefreshToken = Guid.NewGuid().ToString(), // You can replace with real refresh token logic
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                RoleList = roles,
                FullName = user.UserName, // Replace with your user FullName if stored in profile
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

    private string GenerateToken(IdentityUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
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
