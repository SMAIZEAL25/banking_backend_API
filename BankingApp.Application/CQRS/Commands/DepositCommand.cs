
using AutoMapper;
using BankingApp.Application.Response;

public class DepositCommand : IRequest<CustomResponse<DepositResponseDto>>
{
    public string AccountNumber { get; set; }
    public decimal Amount { get; set; }
}

public class DepositCommandHandler : IRequestHandler<DepositCommand, CustomResponse<DepositResponseDto>>
{
    private readonly IBankingService _bankingService;
    private readonly IMapper _mapper;

    public DepositCommandHandler(IBankingService bankingService, IMapper mapper)
    {
        _bankingService = bankingService;
        _mapper = mapper;
    }

    public async Task<CustomResponse<DepositResponseDto>> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        var account = await _bankingService.DepositAsync(request.AccountNumber, request.Amount);
        var dto = _mapper.Map<DepositResponseDto>(account);
        dto.Message = $"Deposit successful. New balance: {account.CurrentBalance} {account.Currency}";

        return CustomResponse<DepositResponseDto>.Success(dto, dto.Message, StatusCodes.Status201Created);
    }
}
