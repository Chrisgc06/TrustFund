using TrustFund.Services.Dtos.Payment;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrustFund.Services.Contracts
{
    public interface IPaymentService
    {
        Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto);
        Task<PaymentDto?> GetPaymentByIdAsync(Guid id); 
        Task<IEnumerable<PaymentDto>> GetPaymentsByLoanIdAsync(Guid loanId);
    }
}