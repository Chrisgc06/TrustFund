using System;
using System.ComponentModel.DataAnnotations;

namespace TrustFund.Services.Dtos.Loan
{
    public class CreateLoanDto
    {
        [Required(ErrorMessage = "Lender ID is required.")]
        public Guid LenderId { get; set; }

        [Required(ErrorMessage = "Borrower ID is required.")]
        public Guid BorrowerId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Interest rate is required.")]
        [Range(0.0, 1.0, ErrorMessage = "Interest rate must be between 0 and 1 (e.g., 0.05 for 5%).")]
        public decimal InterestRate { get; set; }

        [Required(ErrorMessage = "Due date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime DueDate { get; set; }
    }
}