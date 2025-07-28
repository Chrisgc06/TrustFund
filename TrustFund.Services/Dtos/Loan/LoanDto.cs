using System;
using TrustFund.Domain.Entities;

namespace TrustFund.Services.Dtos.Loan
{
    public class LoanDto
    {
        public Guid Id { get; set; }
        public Guid LenderId { get; set; }
        public string? LenderName { get; set; }
        public Guid BorrowerId { get; set; }
        public string? BorrowerName { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal OutstandingAmount { get; set; }
        public LoanStatus Status { get; set; }
    }
}