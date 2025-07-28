using Microsoft.EntityFrameworkCore;
using TrustFund.Domain.Core;
using TrustFund.Domain.Repositories;
using TrustFund.Infrastructure.Context; 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrustFund.Infrastructure.Core
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly TrustFundDbContext _context;
        protected readonly DbSet<TEntity> _dbSet; 

        public BaseRepository(TrustFundDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>(); 
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync(); 
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}