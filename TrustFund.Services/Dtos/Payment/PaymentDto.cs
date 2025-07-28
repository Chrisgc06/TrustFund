using System;

namespace TrustFund.Services.Dtos.Payment
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public string? LoanDescription { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}