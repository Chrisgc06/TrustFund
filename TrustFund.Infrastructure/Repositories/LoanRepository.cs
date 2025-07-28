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
    public class LoanRepository : BaseRepository<Loan>, ILoanRepository
    {
        public LoanRepository(TrustFundDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Loan>> GetLoansByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(l => l.LenderId == userId || l.BorrowerId == userId)
                .Include(l => l.Lender)
                .Include(l => l.Borrower)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
        {
            return await _dbSet
                .Where(l => l.Status == LoanStatus.Pending && l.DueDate < DateTime.Today)
                .Include(l => l.Lender)
                .Include(l => l.Borrower)
                .ToListAsync();
        }
    }
}