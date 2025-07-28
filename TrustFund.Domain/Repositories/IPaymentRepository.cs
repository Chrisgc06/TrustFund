using TrustFund.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrustFund.Domain.Repositories
{
    public interface IPaymentRepository : IBaseRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetPaymentsByLoanIdAsync(Guid loanId);
    }
}