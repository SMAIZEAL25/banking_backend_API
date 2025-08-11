using AutoMapper;
using BankingApp.Application.DTOs;
using BankingApp.Application.Response;
using BankingApp.Domain.Enums;
using BankingApp.Application.Interfaces;
using BankingApp.Infrastruture.Integration.Response;
using BankingApp.Infrastruture.Redis;
using BankingApp.Infrastruture.Services.Interfaces;
using System.Transactions;

namespace BankingApp.Infrastruture.Services
{
    public class AccountTransfer : IAccountTransfer
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IMapper _mapper;

        private const string CachePrefix = "account_";
        private const int CacheExpiryHours = 20;
        private readonly TimeSpan _cacheExpiry = TimeSpan.FromHours(CacheExpiryHours);

        public AccountTransfer(
            IUnitOfWork unitOfWork,
            ICacheService cache,
            IPaymentGateway paymentGateway,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _paymentGateway = paymentGateway;
            _mapper = mapper;
        }

        private static string GetCacheKey(string accountNumber)
            => $"{CachePrefix}{accountNumber}";

        public async Task<CustomResponse<TransferResult>> AccountTransferAsync(TransferRequestDto dto)
        {
            var sourceAccount = await _unitOfWork.BankingService.GetAccountCachedAsync(dto.AccountNumber);
            var destinationAccount = await _unitOfWork.BankingService.GetAccountCachedAsync(dto.BeneficiaryAccountNumber.ToString());

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Internal transfer
            if (destinationAccount != null)
            {
                sourceAccount.CurrentBalance -= dto.Amount;
                destinationAccount.CurrentBalance += dto.Amount;

                await _unitOfWork.Accounts.UpdateAsync(sourceAccount);
                await _unitOfWork.Accounts.UpdateAsync(destinationAccount);

                await _unitOfWork.BankingService.CacheAccountDetails(sourceAccount);
                await _unitOfWork.BankingService.CacheAccountDetails(destinationAccount);

                await _unitOfWork.CommitAsync();
                transaction.Complete();

                return CustomResponse<TransferResult>.Success(new TransferResult
                {
                    IsSuccesTransfer = true,
                    SourceAccount = sourceAccount,
                    BeneficaryAccount = destinationAccount,
                    Message = $"Transfer completed. New balance: {sourceAccount.CurrentBalance:N2}"
                });
            }

            // External transfer via Payment Gateway
            sourceAccount.CurrentBalance -= dto.Amount;
            await _unitOfWork.Accounts.UpdateAsync(sourceAccount);
            await _unitOfWork.BankingService.CacheAccountDetails(sourceAccount);

            var paymentResult = await _paymentGateway.InitializeTransaction(new PaymentRequest
            {
                Amount = dto.Amount,
                Email = sourceAccount.Email ?? "no-reply@bank.local",
                Reference = Guid.NewGuid().ToString("N"),
                CallbackUrl = "https://yourapp.com/payment/callback",
                Metadata = new
                {
                    SourceAccount = dto.AccountNumber,
                    BeneficiaryAccount = dto.BeneficiaryAccountNumber,
                    BeneficiaryBank = dto.BeneficiaryBank
                }
            });

            if (paymentResult != null && paymentResult.Status)
            {
                await _unitOfWork.CommitAsync();
                transaction.Complete();
                return CustomResponse<TransferResult>.Success(new TransferResult
                {
                    IsSuccesTransfer = false,
                    SourceAccount = sourceAccount,
                    Description = paymentResult.Data.Reference,
                    AuthorizationUrl = paymentResult.Data.AuthorizationUrl,
                    Message = $"Transfer processed. New balance: {sourceAccount.CurrentBalance:N2}",
                    Timestamp = DateTime.UtcNow
                });
            }

            // Revert on failure
            await _unitOfWork.BankingService.RevertTransfer(sourceAccount, dto.Amount);
            return CustomResponse<TransferResult>.FailedDependency("External transfer failed. Amount credited back");
        }
    }
}
