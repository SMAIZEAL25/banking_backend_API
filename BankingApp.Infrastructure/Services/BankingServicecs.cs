// Infrastructure/Services/BankingService.cs
using AutoMapper;
using BankingApp.Domain.Entities;
using BankingApp.Application.Interfaces;
using BankingApp.Infrastruture.Redis;
using Microsoft.Extensions.Logging;

public class BankingService : IBankingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;
    private readonly IMapper _mapper;
    private readonly ILogger<BankingService> _logger;

    private const string CachePrefix = "account_";
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromHours(20);

    public BankingService(
        IUnitOfWork unitOfWork,
        ICacheService cache,
        IMapper mapper,
        ILogger<BankingService> logger)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
        _mapper = mapper;
        _logger = logger;
    }

    private static string GetCacheKey(string accountNumber) => $"{CachePrefix}{accountNumber}";

    public async Task<Account> DepositAsync(string accountNumber, decimal amount)
    {
        var account = await GetAccountCachedAsync(accountNumber);
        account.CurrentBalance += amount;

        await _unitOfWork.Accounts.UpdateAsync(account);
        await _unitOfWork.CommitAsync();
        await CacheAccountDetails(account);

        _logger.LogInformation("Deposited {Amount:N2} into account {AccountNumber}. New balance: {Balance:N2}",
            amount, accountNumber, account.CurrentBalance);

        return account;
    }

    public async Task<Account> WithdrawAsync(string accountNumber, decimal amount)
    {
        var account = await GetAccountCachedAsync(accountNumber);
        account.CurrentBalance -= amount;

        await _unitOfWork.Accounts.UpdateAsync(account);
        await _unitOfWork.CommitAsync();
        await CacheAccountDetails(account);

        _logger.LogInformation("Withdrew {Amount:N2} from account {AccountNumber}. New balance: {Balance:N2}",
            amount, accountNumber, account.CurrentBalance);

        return account;
    }

    public async Task<Account> GetAccountCachedAsync(string accountNumber)
    {
        var cacheKey = GetCacheKey(accountNumber);

        var cachedAccount = await _cache.GetAsync<Account>(cacheKey);
        if (cachedAccount != null)
        {
            _logger.LogInformation("Cache hit for account {AccountNumber}", accountNumber);
            return cachedAccount;
        }

        var account = await _unitOfWork.Accounts.GetAccountByNumberAsync(accountNumber);
        if (account != null)
        {
            await CacheAccountDetails(account);
            _logger.LogInformation("Account {AccountNumber} fetched from DB and cached", accountNumber);
        }

        return account;
    }

    public async Task CacheAccountDetails(Account account)
    {
        var cacheKey = GetCacheKey(account.AccountNumber);
        await _cache.SetAsync(cacheKey, account, _cacheExpiry);

        _logger.LogInformation("Account {AccountNumber} cached successfully", account.AccountNumber);
    }

    public async Task RevertTransfer(Account account, decimal amount)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account));

        account.CurrentBalance += amount;
        await _unitOfWork.Accounts.UpdateAsync(account);
        await _unitOfWork.CommitAsync();
        await CacheAccountDetails(account);

        _logger.LogInformation("Reverted transfer of {Amount:N2} to account {AccountNumber}. New balance: {Balance:N2}",
            amount, account.AccountNumber, account.CurrentBalance);
    }
}
