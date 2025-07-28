using TrustFund.Domain.Entities;
using System.Threading.Tasks;

namespace TrustFund.Domain.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmailAsync(string email);

    }
}