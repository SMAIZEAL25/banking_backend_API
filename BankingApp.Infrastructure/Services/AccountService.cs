using AutoMapper;
using BankingApp.Application.DTOs;
using BankingApp.Domain.Entities;
using BankingApp.Domain.Enums;
using BankingApp.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Infrastruture.Services
{
    public class AccountService : IAccountServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;
        private readonly IDistributedCache _distributedCache;
        private int _savingsSequence = 1000;
        private int _otherAccountSequence = 10000;

        private const string BranchCacheKey = "static_branch_lagos_main";
        private const string OfficerCacheKey = "static_account_officer_default";
        private const decimal MinimumOpeningBalance = 1000m;

        public AccountService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AccountService> logger,
            IDistributedCache distributedCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<Account?> OpenAccountAsync(UserDTO userDto, AccountType accountType)
        {
            // Check if user already exists by email
            var existingUser = await _unitOfWork.Users
        .Query()
        .FirstOrDefaultAsync(u => u.Email == userDto.Email);

            User userEntity;
            if (existingUser != null)
            {
                userEntity = existingUser; // Avoid creating new userId
            }
            else
            {
                userEntity = _mapper.Map<User>(userDto);
                await _unitOfWork.Users.AddAsync(userEntity);
            }

            var branchName = await GetOrCreateStaticCacheAsync(BranchCacheKey, "Lagos Main Branch");
            var accountOfficer = await GetOrCreateStaticCacheAsync(OfficerCacheKey, "Officer123");

            var accountNumber = GenerateAccountNumber(accountType);

            var account = new Account
            {
                UserId = userEntity.UserId,
                AccountNumber = accountNumber,
                AccountType = accountType,
                CurrentBalance = MinimumOpeningBalance,
                Currency = CurrencyTypes.NAIRA,
                OpeningDate = DateTime.UtcNow,
                BranchName = branchName,
                AccountStatus = AccountStatus.Active,
                AccountOfficer = accountOfficer,
            };

            await _unitOfWork.Accounts.AddAsync(account);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Account {AccountNumber} opened for user {UserId}", accountNumber, userEntity.UserId);

            return account;
        }

        private async Task<string> GetOrCreateStaticCacheAsync(string cacheKey, string defaultValue)
        {
            var cachedValue = await _distributedCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedValue))
                return cachedValue;

            await _distributedCache.SetStringAsync(cacheKey, defaultValue);
            return defaultValue;
        }

        private string GenerateAccountNumber(AccountType accountType)
        {
            var now = DateTime.Now;
            if (accountType == AccountType.Savings)
                return now.ToString("ddMM") + now.ToString("yy") + (_savingsSequence++).ToString("D4");
            else
                return now.ToString("yyyydd") + (_otherAccountSequence++).ToString("D5");
        }
    }
}
