
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Infrastruture.Repostries.IRepositories
{
    public interface IRepository <T> where T : class
    {
              
        Task <T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<T>> GetByIdAsync(int? id);   
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Query();
    }
}
