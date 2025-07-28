using TrustFund.Domain.Entities;
using TrustFund.Domain.Repositories;
using TrustFund.Infrastructure.Context;
using TrustFund.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrustFund.Infrastructure.Repositories
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(TrustFundDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByLoanIdAsync(Guid loanId)
        {
            return await _dbSet.Where(p => p.LoanId == loanId).ToListAsync();
        }
    }
}