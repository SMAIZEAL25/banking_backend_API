// BankingApp.Application/Handlers/GetAccountTransactionHistoryQueryHandler.cs
using BankingApp.Application.DTOs;
using BankingApp.Application.Queries;
using BankingApp.Application.Response;
using MediatR;

namespace BankingApp.Application.Handlers
{
    public class GetAccountTransactionHistoryQueryHandler
        : IRequestHandler<GetAccountTransactionHistoryQuery, CustomResponse<IEnumerable<TransactionHistoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAccountTransactionHistoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomResponse<IEnumerable<TransactionHistoryDto>>> Handle(
            GetAccountTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.AccountingHistory
                .GetAccountTransactionHistoryAsync(request.AccountNumber);
        }
    }

    public class GetMonthlyTransactionStatementQueryHandler
        : IRequestHandler<GetMonthlyTransactionStatementQuery, CustomResponse<IEnumerable<TransactionHistoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMonthlyTransactionStatementQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomResponse<IEnumerable<TransactionHistoryDto>>> Handle(
            GetMonthlyTransactionStatementQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.AccountingHistory
                .GetMonthlyTransactionStatementAsync(request.AccountNumber, request.NumberOfMonths);
        }
    }
}
