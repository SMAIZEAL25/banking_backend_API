using BankingApp.Application.DTOs;
using BankingApp.Application.Response;
using BankingApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.Interfaces
{
    public interface IAccountingHistoryRepository
    {
        Task<CustomResponse<IEnumerable<TransactionHistoryDto>>> GetAccountTransactionHistoryAsync(string accountNumber);
        Task<CustomResponse<IEnumerable<TransactionHistoryDto>>> GetMonthlyTransactionStatementAsync(string accountNumber, int numberOfMonths);
    }
}
