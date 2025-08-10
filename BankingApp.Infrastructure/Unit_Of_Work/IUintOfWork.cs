// IUnitOfWork.cs
using BankingApp.Domain.Entities;
using BankingApp.Infrastruture.Repostries.IRepositories;
using BankingApp.Infrastruture.Services.Interface;
using System.Threading.Tasks;

namespace BankingApp.Infrastruture.Database
{
    public interface IUnitOfWork
    {
        // Generic Repositories
        IRepository<User> Users { get; }
        IRepository<Transaction> Transactions { get; }

        // Specific Repositories
        IAccountRepository Accounts { get; }

        // Services
        IBankingService BankingService { get; }
        IAccountServices AccountServices { get; }
        IViewAccountBalance ViewAccountBalance { get; }
        IAccountingHistoryRepository GetAccountingHistory { get; }

        Task<int> CommitAsync();
    }
}
