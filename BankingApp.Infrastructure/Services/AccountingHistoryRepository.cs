
using AutoMapper;
using BankingApp.Application.DTOs;
using BankingApp.Application.Interfaces;
using BankingApp.Application.Response;
using BankingApp.Domain.Entities;
using BankingApp.Domain.Enums;
using BankingApp.Infrastructure.Persistence;
using BankingApp.Infrastructure.StoreProcedures;
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

        public async Task<CustomResponse<IEnumerable<TransactionHistoryDto>>> GetMonthlyTransactionStatementAsync(string accountNumber, int numberOfMonths)
        {
            try
            {
                if (numberOfMonths <= 0)
                {
                    _logger.LogWarning("Invalid month count {Months} requested for account {AccountNumber}",
                        numberOfMonths, accountNumber);
                    return CustomResponse<IEnumerable<TransactionHistoryDto>>.BadRequest("Number of months must be positive");
                }

                _logger.LogInformation("Fetching {Months}-month transaction statement for account {AccountNumber}",
                    numberOfMonths, accountNumber);

                var account = await _context.Set<Account>()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(a => a.AccountNumber == accountNumber);

                if (account == null)
                {
                    _logger.LogWarning("Account {AccountNumber} not found", accountNumber);
                    return CustomResponse<IEnumerable<TransactionHistoryDto>>.NotFound("Account not found");
                }

                if (account.AccountStatus == AccountStatus.Closed)
                {
                    _logger.LogWarning("Account {AccountNumber} is closed", accountNumber);
                    return CustomResponse<IEnumerable<TransactionHistoryDto>>.Forbidden("This account is closed");
                }

                var startDate = DateTime.UtcNow.AddMonths(-numberOfMonths);

                List<Transaction> rawTransactionResults;
                try
                {
                    rawTransactionResults = await ExecuteStoredProcedureForMonthlyStatements(accountNumber, startDate);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to execute stored procedure for account {AccountNumber}", accountNumber);
                    return CustomResponse<IEnumerable<TransactionHistoryDto>>.ServerError("Failed to retrieve transactions");
                }

                var transactionResults = _mapper.Map<List<TransactionHistoryDto>>(rawTransactionResults);

                foreach (var dto in transactionResults)
                {
                    dto.AccountName = account.AccountName;
                    dto.AccountNumber = accountNumber; // Ensure account number is included
                }

                if (!transactionResults.Any())
                {
                    _logger.LogInformation("No transactions found for account {AccountNumber} in last {Months} months",
                        accountNumber, numberOfMonths);
                    return CustomResponse<IEnumerable<TransactionHistoryDto>>.Success(
                        transactionResults,
                        $"No transactions found for the past {numberOfMonths} months");
                }

                _logger.LogInformation("Successfully retrieved {Count} transactions for account {AccountNumber}",
                    transactionResults.Count, accountNumber);

                return CustomResponse<IEnumerable<TransactionHistoryDto>>.Success(transactionResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching transactions for account {AccountNumber}",
                    accountNumber);
                return CustomResponse<IEnumerable<TransactionHistoryDto>>.ServerError("An unexpected error occurred");
            }
        }

        //GetAccout Trasaction History For only 5 weeks
        public async Task<CustomResponse<IEnumerable<TransactionHistoryDto>>> GetAccountTransactionHistoryAsync(string accountNumber)
        {
            try
            {
                const int numberOfWeeks = 5;
                _logger.LogInformation("Fetching {Weeks}-week transaction history for account {AccountNumber}",
                    numberOfWeeks, accountNumber);

                var account = await _context.Set<Account>()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(a => a.AccountNumber == accountNumber);

                if (account == null)
                {
                    _logger.LogWarning("Account {AccountNumber} not found", accountNumber);
                    return CustomResponse<IEnumerable<TransactionHistoryDto>>.NotFound("Account not found");
                }

                if (account.AccountStatus == AccountStatus.Closed)
                {
                    _logger.LogWarning("Account {AccountNumber} is closed", accountNumber);
                    return CustomResponse<IEnumerable<TransactionHistoryDto>>.Forbidden("This account is closed");
                }

                var startDate = DateTime.UtcNow.AddDays(-(numberOfWeeks * 7));

                List<Transaction> rawTransactionResults;
                try
                {
                    rawTransactionResults = await ExecuteStoredProcedureForWeeklyStatements(accountNumber, startDate);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to execute stored procedure for account {AccountNumber}", accountNumber);
                    return CustomResponse<IEnumerable<TransactionHistoryDto>>.ServerError("Failed to retrieve transactions");
                }

                var transactionResults = _mapper.Map<List<TransactionHistoryDto>>(rawTransactionResults);

                foreach (var dto in transactionResults)
                {
                    dto.AccountName = account.AccountName;
                    dto.AccountNumber = accountNumber; // Ensure account number is included
                }

                if (!transactionResults.Any())
                {
                    _logger.LogInformation("No transactions found for account {AccountNumber} in last {Weeks} weeks",
                        accountNumber, numberOfWeeks);
                    return CustomResponse<IEnumerable<TransactionHistoryDto>>.Success(
                        transactionResults,
                        $"No transactions found for the past {numberOfWeeks} weeks");
                }

                _logger.LogInformation("Successfully retrieved {Count} transactions for account {AccountNumber}",
                    transactionResults.Count, accountNumber);

                return CustomResponse<IEnumerable<TransactionHistoryDto>>.Success(transactionResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching transactions for account {AccountNumber}", accountNumber);
                return CustomResponse<IEnumerable<TransactionHistoryDto>>.ServerError("An unexpected error occurred");
            }
        }

        private async Task<List<Transaction>> ExecuteStoredProcedureForWeeklyStatements(string accountNumber, DateTime startDate)
        {
            _logger.LogDebug("Executing weekly transaction stored procedure for account {AccountNumber} from {StartDate}",
                accountNumber, startDate);

            return await _context.Set<Transaction>()
                .FromSqlRaw($"{StoredProcedureScripts.GetWeeklyTransactionStatements} EXEC GetWeeklyTransactionStatements @AccountNumber = {{0}}, @StartDate = {{1}}",
                            accountNumber, startDate)
                .ToListAsync();
        }

        private async Task<List<Transaction>> ExecuteStoredProcedureForMonthlyStatements(string accountNumber, DateTime startDate)
        {
            _logger.LogDebug("Executing monthly transaction stored procedure for account {AccountNumber} from {StartDate}",
                accountNumber, startDate);

            return await _context.Set<Transaction>()
                .FromSqlRaw("EXEC GetMonthlyTransactionStatements @AccountNumber = {0}, @StartDate = {1}",
                            accountNumber, startDate)
                .ToListAsync();
        }

    }
}
