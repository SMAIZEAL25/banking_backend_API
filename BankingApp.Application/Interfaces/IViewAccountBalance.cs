using banking_backend_API_By_Solomon_Chika_Samuel.Response;
using BankingApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.Interfaces
{
    public interface IViewAccountBalance
    {
        Task<AccountBalanceResponseDto> ViewAccountBalanceAsync(string accountNumber);
        Task<AccountDetailsResponseDto> ViewAccountDetailsAsync(string accountNumber);
    }
}
