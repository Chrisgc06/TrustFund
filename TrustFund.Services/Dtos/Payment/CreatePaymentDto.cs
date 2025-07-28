using System;
using System.ComponentModel.DataAnnotations;

namespace TrustFund.Services.Dtos.Payment
{
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "Loan ID is required.")]
        public Guid LoanId { get; set; }

        [Required(ErrorMessage = "Amount paid is required.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount paid must be greater than zero.")]
        public decimal AmountPaid { get; set; }
    }
}