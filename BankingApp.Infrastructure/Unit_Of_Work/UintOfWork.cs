// UnitOfWork.cs
using BankingApp.Domain.Entities;
using BankingApp.Infrastructure.Persistence;
using BankingApp.Infrastruture.Repostries.IRepositories;
using BankingApp.Infrastruture.Services.Interface;

namespace BankingApp.Infrastruture.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BankingDbContext _context;

        public IRepository<User> Users { get; }
        public IRepository<Transaction> Transactions { get; }
        public IAccountRepository Accounts { get; }

        public IBankingService BankingService { get; }
        public IAccountServices AccountServices { get; }
        public IViewAccountBalance ViewAccountBalance { get; }
        public IAccountingHistoryRepository GetAccountingHistory { get; }

        public UnitOfWork(
            BankingDbContext context,
            IRepository<User> userRepo,
            IRepository<Transaction> transactionRepo,
            IAccountRepository accountRepo,
            IBankingService bankingService,
            IAccountServices accountServices,
            IViewAccountBalance viewAccountBalance,
            IAccountingHistoryRepository getAccountingHistory)
        {
            _context = context;
            Users = userRepo;
            Transactions = transactionRepo;
            Accounts = accountRepo;

            BankingService = bankingService;
            AccountServices = accountServices;
            ViewAccountBalance = viewAccountBalance;
            GetAccountingHistory = getAccountingHistory;
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
