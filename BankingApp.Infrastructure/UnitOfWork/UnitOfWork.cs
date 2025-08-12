using BankingApp.Application.Interfaces;
using BankingApp.Domain.Entities;
using BankingApp.Infrastructure.Persistence;
using BankingApp.Infrastruture.Repostries.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Infrastructure.UnitOfWork
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly BankingDbContext _context;

        public IRepository<User> Users { get; }
        public IRepository<Transaction> Transactions { get; }
        public IRepository<Account> Account { get; }
        public IAccountRepository Accounts { get; }

        public IBankingService BankingService { get; }
        public IAccountServices AccountServices { get; }
        public IViewAccountBalance ViewAccountBalance { get; }
        public IAccountingHistoryRepository GetAccountingHistory { get; }

        public UnitOfWork(
            BankingDbContext context,
            IRepository<User> userRepo,
            IRepository<Transaction> transactionRepo,
            IRepository<Account> Accountrepo,
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
            Account = Accountrepo;
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

