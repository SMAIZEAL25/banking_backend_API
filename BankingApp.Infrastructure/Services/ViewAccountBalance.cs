using AutoMapper;
using BankingApp.Application.DTOs;
using BankingApp.Domain.Entities;
using BankingApp.Domain.Enums;
using BankingApp.Infrastruture.Redis;
using BankingApp.Infrastruture.Repostries.IRepositories;
using BankingApp.Infrastruture.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;

public class ViewAccountBalance : IViewAccountBalance
{
    private readonly IRepository<Account> _accountRepository;
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger<ViewAccountBalance> _logger;
    private const string CachePrefix = "account_";
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromHours(1);

    public ViewAccountBalance(
        IRepository<Account> accountRepository,
        IRepository<Transaction> transactionRepository,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<ViewAccountBalance> logger)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _cacheService = cacheService;
        _mapper = mapper;
        _logger = logger;
    }

    private string GetCacheKey(string accountNumber) => $"{CachePrefix}{accountNumber}";

    public async Task<AccountBalanceResponseDto> ViewAccountBalanceAsync(string accountNumber)
    {
        _logger.LogInformation("Attempting to view account balance for AccountNumber: {AccountNumber}", accountNumber);

        try
        {
            var cacheKey = GetCacheKey(accountNumber);
            var cachedAccount = await _cacheService.GetAsync<Account>(cacheKey);

            if (cachedAccount != null)
            {
                _logger.LogDebug("Account balance retrieved from cache for {AccountNumber}", accountNumber);
                return new AccountBalanceResponseDto
                {
                    AccountNumber = cachedAccount.AccountNumber,
                    CurrentBalance = cachedAccount.CurrentBalance,
                    AccountStatus = cachedAccount.AccountStatus,
                    Message = "Account balance retrieved successfully from cache.",
                    StatusCode = StatusCodes.Status200OK
                };
            }

            // If not in cache, get from database
            var account = await _accountRepository.GetSingleOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
            {
                _logger.LogWarning("Account not found for AccountNumber: {AccountNumber}", accountNumber);
                return new AccountBalanceResponseDto
                {
                    Message = "Account not found.",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            // Cache the account for future requests
            await _cacheService.SetAsync(cacheKey, account, _cacheExpiry);

            if (account.AccountStatus == AccountStatus.Closed)
            {
                _logger.LogWarning("Attempt to view balance of a closed account: {AccountNumber}", accountNumber);
                return new AccountBalanceResponseDto
                {
                    AccountNumber = account.AccountNumber,
                    CurrentBalance = account.CurrentBalance,
                    AccountStatus = account.AccountStatus,
                    Message = "This account is closed. Please contact support.",
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            return new AccountBalanceResponseDto
            {
                AccountNumber = account.AccountNumber,
                CurrentBalance = account.CurrentBalance,
                AccountStatus = account.AccountStatus,
                Message = account.AccountStatus == AccountStatus.Active
                    ? "Account balance retrieved successfully."
                    : $"Account is currently {account.AccountStatus}.",
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving balance for account {AccountNumber}", accountNumber);
            return new AccountBalanceResponseDto
            {
                Message = "An error occurred while retrieving account balance.",
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }


    public async Task<AccountDetailsResponseDto> ViewAccountDetailsAsync(string accountNumber)
    {
        _logger.LogInformation("Attempting to retrieve account details for AccountNumber: {AccountNumber}", accountNumber);

        try
        {
            // Try to get from cache first
            var cacheKey = GetCacheKey(accountNumber);
            var cachedAccount = await _cacheService.GetAsync<Account>(cacheKey);

            if (cachedAccount != null)
            {
                _logger.LogDebug("Account details retrieved from cache for {AccountNumber}", accountNumber);
                return _mapper.Map<AccountDetailsResponseDto>(cachedAccount);
            }

            // If not in cache, get from database
            var accountList = await _accountRepository.GetAllAsync();
            var account = accountList.FirstOrDefault(a => a.AccountNumber == accountNumber);

            if (account == null)
            {
                _logger.LogWarning("Account with AccountNumber {AccountNumber} does not exist.", accountNumber);
                return new AccountDetailsResponseDto
                {
                    Message = "Account does not exist.",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            // Cache the account
            await _cacheService.SetAsync(cacheKey, account, _cacheExpiry);

            if (account.AccountStatus == AccountStatus.Closed)
            {
                _logger.LogWarning("Account with AccountNumber {AccountNumber} is closed.", accountNumber);
                return new AccountDetailsResponseDto
                {
                    AccountNumber = account.AccountNumber,
                    Message = "Account is closed. Please contact the IT Operations Department for details.",
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            // Just map and return
            return _mapper.Map<AccountDetailsResponseDto>(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving details for account {AccountNumber}", accountNumber);
            return new AccountDetailsResponseDto
            {
                Message = "An error occurred while retrieving account details.",
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

}