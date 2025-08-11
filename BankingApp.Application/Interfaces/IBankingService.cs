using BankingApp.Application.DTOs;
using BankingApp.Application.Response;
using BankingApp.Domain.Entities;
using BankingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.Interfaces
{
    public interface IBankingService
    {
        Task CacheAccountDetails(Account account);
        Task<Account> DepositAsync(string accountNumber, decimal amount);
        Task<Account> GetAccountCachedAsync(string accountNumber);
        Task RevertTransfer(Account account, decimal amount);
        Task<Account> WithdrawAsync(string accountNumber, decimal amount);
    }

}
