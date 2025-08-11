using AutoMapper;
using AutoMapper.QueryableExtensions;
using BankingApp.Domain.Entities;
using BankingApp.Infrastructure.Persistence;
using BankingApp.Infrastruture.Repostries.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;



namespace BankingApp.Infrastruture.Repostries.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly BankingDbContext _bankingDb;
        private readonly IMapper _mapper;

        public Repository(BankingDbContext bankingDb, IMapper mapper)
        {
            _bankingDb = bankingDb;
            _mapper = mapper;
        }
        public async Task<T> AddAsync(T entity)
        {
            await _bankingDb.AddAsync(entity);
            await _bankingDb.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int Id)
        {
            var entity = await GetByIdAsync(Id);
            _bankingDb.Set<IEnumerable<T>>().Remove(entity);
            await _bankingDb.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetByIdAsync(int? id)
        {
            if (id is null)
            {
                return null;
            }

            var response = await _bankingDb.Set<IEnumerable<T>>().FindAsync(id);

            return response;
        }        


        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _bankingDb.Set<T>().ToListAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _bankingDb.Update(entity);
            await _bankingDb.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }

        public async Task<T> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _bankingDb.Set<T>().AsNoTracking().SingleOrDefaultAsync(predicate);
        }


    }
}
