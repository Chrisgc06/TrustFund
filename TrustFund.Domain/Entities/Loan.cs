using TrustFund.Domain.Core;
using System;
using System.Collections.Generic;

namespace TrustFund.Domain.Entities
{
    public class Loan : BaseEntity
    {
        public Guid LenderId { get; set; } 
        public Guid BorrowerId { get; set; } 
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; } 
        public decimal OutstandingAmount { get; set; } 
        public LoanStatus Status { get; set; }

        public User Lender { get; set; }
        public User Borrower { get; set; }
        public ICollection<Payment> Payments { get; set; }

        public Loan()
        {
            Payments = new HashSet<Payment>();
            Status = LoanStatus.Pending; 
        }
    }

    public enum LoanStatus
    {
        Pending,   
        Paid,       
        Overdue,    
        Canceled    
    }
}
