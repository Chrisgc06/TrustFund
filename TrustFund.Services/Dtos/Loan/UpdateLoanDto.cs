using TrustFund.Domain.Entities; 
using System;
using System.ComponentModel.DataAnnotations;

namespace TrustFund.Services.Dtos.Loan
{
    public class UpdateLoanDto
    {
        [Range(0.0, (double)decimal.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal? Amount { get; set; } 

        [Range(0.0, 1.0, ErrorMessage = "Interest rate must be between 0 and 1 (e.g., 0.05 for 5%).")]
        public decimal? InterestRate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")] 
        public DateTime? DueDate { get; set; }

        public LoanStatus? Status { get; set; } 
    }
}