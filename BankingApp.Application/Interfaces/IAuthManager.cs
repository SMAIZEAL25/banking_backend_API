using BankingApp.Application.DTOs;
using BankingApp.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BankingApp.Application.Interfaces
{
    public interface IAuthManager
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
      
        Task<RegistrationResponse> RegisterAsync(RegisterRequest request, string role = "Reader");
    }
}
