using BankingApp.Application.DTOs;
using BankingApp.Application.Response;
using BankingApp.Domain.Enums;
using BankingApp.Infrastruture.Services;
using FluentValidation;

namespace BankingApp.Application.Commands
{
    public class OpenAccountCommand
    {
        private readonly IAccountServices _accountService;
        private readonly IValidator<UserDTO> _validator;

        public OpenAccountCommand(IAccountServices accountService, IValidator<UserDTO> validator)
        {
            _accountService = accountService;
            _validator = validator;
        }

        public async Task<CustomResponse<AccountDTO>> ExecuteAsync(UserDTO userDto, AccountType accountType)
        {
            var validationResult = await _validator.ValidateAsync(userDto);
            if (!validationResult.IsValid)
            {
                return CustomResponse<AccountDTO>.BadRequest(
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var account = await _accountService.OpenAccountAsync(userDto, accountType);
            if (account == null)
                return CustomResponse<AccountDTO>.ServerError("Account creation failed");

            return CustomResponse<AccountDTO>.Success(
                new AccountDTO
                {
                    AccountNumber = account.AccountNumber,
                    AccountType = account.AccountType,
                    CurrentBalance = account.CurrentBalance
                },
                "Account opened successfully");
        }
    }
}
