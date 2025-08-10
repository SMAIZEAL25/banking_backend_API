using AutoMapper;
using BankingApp.Application.DTOs;
using BankingApp.Application.Response;
using BankingApp.Domain.Enums;
using BankingApp.Infrastructure.Services.Interfaces;
using BankingApp.Infrastruture.Integration.Response;
using BankingApp.Infrastruture.Redis;
using BankingApp.Infrastruture.Services.Interface;
using BankingApp.Infrastruture.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System.Transactions;

namespace BankingApp.Infrastruture.Services
{
    public class AccountTransfer : IAccountTransfer
    {
        private readonly IAccountRepository _accountRepo;
        private readonly ICacheService _cache;
        private readonly IPaymentGateway _paymentGateway;
        private readonly ILogger<BankingService> _logger;
        private readonly IMapper _mapper;
        private readonly IBankingService _bankingService;

        // Consant
        private const string CachePrefix = "account_";
        private const int CacheExpiryHours = 20;
        private readonly TimeSpan _cacheExpiry = TimeSpan.FromHours(CacheExpiryHours);

        public AccountTransfer(
            IAccountRepository accountRepo,
            ICacheService cache,
            IPaymentGateway paymentGateway,
            ILogger<BankingService> logger,
            IMapper mapper,
            IBankingService bankingService) 
        {
            _accountRepo = accountRepo;
            _cache = cache;
            _paymentGateway = paymentGateway;
            _logger = logger;
            _mapper = mapper;
            _bankingService = bankingService; 
        }

        private static string GetCacheKey(string accountNumber) => $"{CachePrefix}{accountNumber}";

        public async Task<CustomResponse<TransferResult>> AccountTransferAsync(TransferRequestDto transferRequestDto)
        {
            var sourceCacheKey = GetCacheKey(transferRequestDto.AccountNumber);
            var destinationCacheKey = GetCacheKey(transferRequestDto.BeneficiaryAccountNumber.ToString());

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                // 1. Validate Source Account
                var sourceAccount = await _bankingService.GetAccountCachedAsync(transferRequestDto.AccountNumber);
                if (sourceAccount == null)
                    return CustomResponse<TransferResult>.NotFound("Source account not found");

                if (sourceAccount.AccountStatus != AccountStatus.Active)
                    return CustomResponse<TransferResult>.Forbidden("Source account is closed");

                if (sourceAccount.Currency != transferRequestDto.Currency)
                    return CustomResponse<TransferResult>.BadRequest("Currency mismatch");

                if (sourceAccount.CurrentBalance < transferRequestDto.Amount)
                    return CustomResponse<TransferResult>.BadRequest("Insufficient funds");

                // 2. Check Transfer Type
                var destinationAccount = await _bankingService.GetAccountCachedAsync(transferRequestDto.BeneficiaryAccountNumber.ToString());
                bool internalTransfer = destinationAccount != null;

                if (internalTransfer)
                {
                    // 3. Process Internal Transfer
                    if (destinationAccount.Currency != transferRequestDto.Currency)
                        return CustomResponse<TransferResult>.BadRequest("Destination currency mismatch");

                    if (destinationAccount.BeneficiaryBank != transferRequestDto.BeneficiaryBank)
                        return CustomResponse<TransferResult>.BadRequest("Bank mismatch");

                    // Debit Source
                    sourceAccount.CurrentBalance -= transferRequestDto.Amount;
                    await _accountRepo.UpdateAsync(sourceAccount);
                    await _bankingService.CacheAccountDetails(sourceAccount);

                    // Credit Destination
                    destinationAccount.CurrentBalance += transferRequestDto.Amount;
                    await _accountRepo.UpdateAsync(destinationAccount);
                    await _bankingService.CacheAccountDetails(destinationAccount);

                    transaction.Complete();

                    return CustomResponse<TransferResult>.Success(
                        new TransferResult
                        {
                            IsSuccesTransfer = true,
                            SourceAccount = sourceAccount,
                            BeneficaryAccount = destinationAccount,
                            Message = $"Transfer completed. New balance: {sourceAccount.CurrentBalance:N2}"
                        },
                        
                        statusCode: StatusCodes.Status200OK
                    );
                }
                else
                {
                    // Process External Transfer via Paystack
                    try
                    {
                        // Initial Debit
                        sourceAccount.CurrentBalance -= transferRequestDto.Amount;
                        await _accountRepo.UpdateAsync(sourceAccount);
                        await _bankingService.CacheAccountDetails(sourceAccount);

                        var pRequest = new PaymentRequest
                        {
                            Amount = transferRequestDto.Amount, // here my amout is in Naira
                            Email = sourceAccount.Email ?? "no-reply@bank.local",
                            Reference = Guid.NewGuid().ToString("N"),
                            CallbackUrl = "https://yourapp.com/payment/callback",
                            Metadata = new
                            {
                                SourceAccount = transferRequestDto.AccountNumber,
                                BeneficiaryAccount = transferRequestDto.BeneficiaryAccountNumber,
                                BeneficiaryBank = transferRequestDto.BeneficiaryBank
                            }
                        };

                        var paymentResult = await _paymentGateway.InitializeTransaction(pRequest);

                        if (paymentResult != null && paymentResult.Status)
                        {
                            transaction.Complete();

                            _logger.LogInformation("External transfer processed successfully. Reference: {Reference}",
                                paymentResult.Data.Reference);

                            return CustomResponse<TransferResult>.Success(
                                new TransferResult
                                {
                                    IsSuccesTransfer = false,
                                    SourceAccount = sourceAccount,
                                    Description = paymentResult.Data.Reference,
                                    AuthorizationUrl = paymentResult.Data.AuthorizationUrl,
                                    Message = $"Transfer processed. New balance: {sourceAccount.CurrentBalance:N2}",
                                    Timestamp = DateTime.UtcNow
                                },
                                // <-- FIX: pass the status code as a named parameter
                                statusCode: StatusCodes.Status200OK
                            );
                        }
                        else
                        {
                            // revert debit
                            await _bankingService.RevertTransfer(sourceAccount, transferRequestDto.Amount);
                            return CustomResponse<TransferResult>.FailedDependency("External transfer failed. Amount credited back");
                        }
                    }
                    catch (Exception ex)
                    {
                        await _bankingService.RevertTransfer(sourceAccount, transferRequestDto.Amount);
                        _logger.LogError(ex, "Paystack processing error");
                        return CustomResponse<TransferResult>.ServerError("Payment processing error. Amount credited back");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transfer processing error");
                await _cache.RemoveAsync(sourceCacheKey);
                await _cache.RemoveAsync(destinationCacheKey);
                return CustomResponse<TransferResult>.ServerError("Transfer processing failed");
            }
        }
    }
}





