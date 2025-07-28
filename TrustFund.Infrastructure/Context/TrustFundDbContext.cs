using Microsoft.EntityFrameworkCore;
using TrustFund.Domain.Entities; 

namespace TrustFund.Infrastructure.Context
{
    public class TrustFundDbContext : DbContext
    {
        public TrustFundDbContext(DbContextOptions<TrustFundDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Lender) 
                .WithMany(u => u.LoansAsLender) 
                .HasForeignKey(l => l.LenderId) 
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Borrower) 
                .WithMany(u => u.LoansAsBorrower) 
                .HasForeignKey(l => l.BorrowerId) 
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Loan) 
                .WithMany(l => l.Payments) 
                .HasForeignKey(p => p.LoanId) 
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}