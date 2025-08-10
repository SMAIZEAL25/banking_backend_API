using AutoMapper;
using BankingApp.Domain.Entities;
using BankingApp.Infrastructure.Persistence;
using BankingApp.Infrastruture.Database;
using BankingApp.Infrastruture.Repostries.Repository;
using BankingApp.Infrastruture.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Infrastruture.Services
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        private readonly BankingDbContext _context;

        public AccountRepository(BankingDbContext context, IMapper mapper): base(context, mapper)
        {
            _context = context;
        }

        public async Task<Account?> GetAccountByNumberAsync(string accountNumber)
        {
            return await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }
    }

}
