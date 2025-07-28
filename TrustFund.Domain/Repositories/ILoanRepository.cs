using TrustFund.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrustFund.Domain.Repositories
{
    public interface ILoanRepository : IBaseRepository<Loan>
    {
        Task<IEnumerable<Loan>> GetLoansByUserIdAsync(Guid userId);
        Task<IEnumerable<Loan>> GetOverdueLoansAsync();
    }
}