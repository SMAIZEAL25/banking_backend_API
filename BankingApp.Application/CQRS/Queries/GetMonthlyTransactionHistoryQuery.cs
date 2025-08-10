// BankingApp.Application/Queries/GetAccountTransactionHistoryQuery.cs
using BankingApp.Application.DTOs;
using BankingApp.Application.Response;
using MediatR;

namespace BankingApp.Application.Queries
{
    

    public record GetMonthlyTransactionStatementQuery(string AccountNumber, int NumberOfMonths)
        : IRequest<CustomResponse<IEnumerable<TransactionHistoryDto>>>;
}
