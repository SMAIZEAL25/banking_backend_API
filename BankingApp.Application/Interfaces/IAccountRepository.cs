using BankingApp.Domain.Entities;
using BankingApp.Infrastruture.Repostries.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.Interfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account?> GetAccountByNumberAsync(string accountNumber);
    }

}
