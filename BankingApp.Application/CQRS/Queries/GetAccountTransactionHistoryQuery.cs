// BankingApp.Application/Queries/GetAccountTransactionHistoryQuery.cs
using BankingApp.Application.DTOs;
using BankingApp.Application.Response;
using MediatR;

namespace BankingApp.Application.Queries
{
    public record GetAccountTransactionHistoryQuery(string AccountNumber)
        : IRequest<CustomResponse<IEnumerable<TransactionHistoryDto>>>;

    
}
