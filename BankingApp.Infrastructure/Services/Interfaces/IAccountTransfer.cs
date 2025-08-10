using BankingApp.Application.DTOs;
using BankingApp.Application.Response;
using BankingApp.Infrastruture.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Infrastructure.Services.Interfaces
{
    public interface IAccountTransfer
    {
        Task<CustomResponse<TransferResult>> AccountTransferAsync(TransferRequestDto transferRequestDto)
    }
}
