using BankingApp.Application.Interfaces;
using BankingApp.Domain.Entities;
using BankingApp.Infrastruture.Repostries.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Transaction> Transactions { get; }
        IAccountRepository Accounts { get; }

        IBankingService BankingService { get; }
        IAccountServices AccountServices { get; }
        IViewAccountBalance ViewAccountBalance { get; }
        IAccountingHistoryRepository GetAccountingHistory { get; }

        Task<int> CommitAsync();
    }
}
