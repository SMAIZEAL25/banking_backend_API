using BankingApp.Application.Response;
using BankingApp.Infrastruture.Services;
using BankingApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

public class AccountTransferCommandHandler
    : IRequestHandler<AccountTransferCommand, CustomResponse<TransferResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountTransfer _accountTransfer;
    private readonly ILogger<AccountTransferCommandHandler> _logger;

    public AccountTransferCommandHandler(
        IUnitOfWork unitOfWork,
        IAccountTransfer accountTransfer,
        ILogger<AccountTransferCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _accountTransfer = accountTransfer;
        _logger = logger;
    }

    public async Task<CustomResponse<TransferResult>> Handle(
        AccountTransferCommand request,
        CancellationToken cancellationToken)
    {
        // The service just executes business logic, no response formatting
        var result = await _accountTransfer.AccountTransferAsync(request.TransferRequest);

        if (result == null)
            return CustomResponse<TransferResult>.ServerError("Transfer failed due to internal error.");

        return result;
    }
}
