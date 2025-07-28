using TrustFund.Domain.Entities;
using TrustFund.Domain.Repositories;
using TrustFund.Infrastructure.Context;
using TrustFund.Infrastructure.Core; 
using Microsoft.EntityFrameworkCore; 
using System.Threading.Tasks;

namespace TrustFund.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(TrustFundDbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {

            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}