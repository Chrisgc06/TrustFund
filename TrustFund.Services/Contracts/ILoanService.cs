using TrustFund.Services.Dtos.Loan;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrustFund.Services.Contracts
{
    public interface ILoanService
    {
        Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto);
        Task<LoanDto?> GetLoanByIdAsync(Guid id); 
        Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId);
        Task<IEnumerable<LoanDto>> GetOverdueLoansAsync();
        Task UpdateLoanAsync(Guid id, UpdateLoanDto updateLoanDto);
        Task DeleteLoanAsync(Guid id);
        Task<decimal> CalculateInterestAsync(Guid loanId);
    }
}