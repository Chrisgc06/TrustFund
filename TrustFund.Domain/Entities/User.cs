using TrustFund.Domain.Core;
using System.Collections.Generic;

namespace TrustFund.Domain.Entities
{
    public class User : Person
    {
        public ICollection<Loan> LoansAsLender { get; set; }
        public ICollection<Loan> LoansAsBorrower { get; set; }

        public User()
        {
            LoansAsLender = new HashSet<Loan>();
            LoansAsBorrower = new HashSet<Loan>();
        }
    }
}