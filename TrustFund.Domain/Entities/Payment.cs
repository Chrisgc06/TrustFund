using TrustFund.Domain.Core;
using System;

namespace TrustFund.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public Guid LoanId { get; set; } 
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }

        public Loan Loan { get; set; }
    }
}