using BankingApp.Application.DTOs;
using BankingApp.Application.Response;
using BankingApp.Infrastruture.Services;
using MediatR;

public record AccountTransferCommand(TransferRequestDto TransferRequest)
    : IRequest<CustomResponse<TransferResult>>;
