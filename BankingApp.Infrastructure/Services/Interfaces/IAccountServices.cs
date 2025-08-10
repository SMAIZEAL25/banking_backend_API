using banking_backend_API_By_Solomon_Chika_Samuel.Response;
using BankingApp.Application.DTOs;
using BankingApp.Domain.Entities;
using BankingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Infrastruture.Services.Interface
{
    public interface IAccountServices
    {
        Task<Account?> OpenAccountAsync(UserDTO userDto, AccountType accountType);
    }
}
