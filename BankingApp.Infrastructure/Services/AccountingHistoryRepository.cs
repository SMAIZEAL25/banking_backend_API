// BankingApp.Infrastructure/Repositories/AccountingHistoryRepository.cs
using AutoMapper;
using BankingApp.Application.DTOs;
using BankingApp.Application.Response;
using BankingApp.Domain.Entities;
using BankingApp.Domain.Enums;
using BankingApp.Infrastructure.Persistence;
using BankingApp.Infrastruture.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankingApp.Infrastructure.Repositories
{
    public class AccountingHistoryRepository : IAccountingHistoryRepository
    {
        private readonly DbContext _context;
        private readonly ILogger<AccountingHistoryRepository> _logger;
        private readonly IMapper _mapper;

        public AccountingHistoryRepository (DbContext context, ILogger<AccountingHistoryRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<CustomResponse<IEnumerable<TransactionHistoryDto>>> GetAccountTransactionHistoryAsync(string accountNumber)
        {
            _logger.LogInformation("Fetching transaction history for account {AccountNumber}", accountNumber);

            var account = await _context.Set<Account>()
                .Include(a => a.Transactions)
                .ThenInclude(t => t.User)
                .AsNoTracking()
                .SingleOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
            {
                _logger.LogWarning("Account {AccountNumber} not found", accountNumber);
                return CustomResponse<IEnumerable<TransactionHistoryDto>>.Fail("Account not found.");
            }

            if (account.AccountStatus == AccountStatus.Closed)
            {
                _logger.LogWarning("Account {AccountNumber} is closed", accountNumber);
                return CustomResponse<IEnumerable<TransactionHistoryDto>>.Fail("This account is closed.");
            }

            var threeWeeksAgo = DateTime.UtcNow.AddDays(-21);

            var transactions = account.Transactions
                .Where(t => t.TransactionDate >= threeWeeksAgo)
                .ToList();

            var transactionDtos = _mapper.Map<List<TransactionHistoryDto>>(transactions);

            foreach (var dto in transactionDtos)
            {
                dto.AccountName = account.AccountName;
            }

            _logger.LogInformation("Retrieved {Count} transactions for account {AccountNumber}", transactionDtos.Count, accountNumber);

            return CustomResponse<IEnumerable<TransactionHistoryDto>>.Success(transactionDtos);
        }

        public async Task<CustomResponse<IEnumerable<TransactionHistoryDto>>> GetMonthlyTransactionStatementAsync(string accountNumber, int numberOfMonths)
        {
            _logger.LogInformation("Fetching {Months}-month transaction statement for account {AccountNumber}", numberOfMonths, accountNumber);

            var account = await _context.Set<Account>()
                .AsNoTracking()
                .SingleOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
            {
                _logger.LogWarning("Account {AccountNumber} not found", accountNumber);
                return CustomResponse<IEnumerable<TransactionHistoryDto>>.Fail("Account not found.");
            }

            if (account.AccountStatus == AccountStatus.Closed)
            {
                _logger.LogWarning("Account {AccountNumber} is closed", accountNumber);
                return CustomResponse<IEnumerable<TransactionHistoryDto>>.Fail("This account is closed.");
            }

            var startDate = DateTime.UtcNow.AddMonths(-numberOfMonths);

            var rawTransactionResults = await ExecuteStoredProcedureForMonthlyStatements(accountNumber, startDate);

            var transactionResults = _mapper.Map<List<TransactionHistoryDto>>(rawTransactionResults);

            foreach (var dto in transactionResults)
            {
                dto.AccountName = account.AccountName;
            }

            if (transactionResults.Count == 0)
            {
                _logger.LogInformation("No transactions found for account {AccountNumber} in last {Months} months", accountNumber, numberOfMonths);
                return CustomResponse<IEnumerable<TransactionHistoryDto>>.Fail($"No records found for the past {numberOfMonths} months.");
            }

            _logger.LogInformation("Retrieved {Count} monthly transactions for account {AccountNumber}", transactionResults.Count, accountNumber);

            return CustomResponse<IEnumerable<TransactionHistoryDto>>.Success(transactionResults);
        }

        private async Task<List<Transaction>> ExecuteStoredProcedureForMonthlyStatements(string accountNumber, DateTime startDate)
        {
            _logger.LogDebug("Executing stored procedure for account {AccountNumber} from {StartDate}", accountNumber, startDate);

            return await _context.Set<Transaction>()
                .FromSqlRaw("EXEC GetMonthlyTransactionStatements @AccountNumber = {0}, @StartDate = {1}", accountNumber, startDate)
                .ToListAsync();
        }
    }
}
