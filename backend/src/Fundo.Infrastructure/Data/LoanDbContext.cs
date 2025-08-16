using Fundo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
namespace Fundo.Infrastructure.Data
{
    public class LoanDbContext : DbContext
    {
        public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options) { }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<LoanHistory> LoanHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure domain naming: domain entities use IdLoan/IdPayment etc.
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Loan)
                .WithMany()
                .HasForeignKey(p => p.IdLoan)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LoanHistory>()
                .HasOne(h => h.Loan)
                .WithMany()
                .HasForeignKey(h => h.IdLoan)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Loan>().HasData(
                new Loan { Id = 1, Amount = 1500.00m, CurrentBalance = 500.00m, ApplicantName = "Maria Silva", Status = "active" },
                new Loan { Id = 2, Amount = 2000.00m, CurrentBalance = 2000.00m, ApplicantName = "João Souza", Status = "active" },
                new Loan { Id = 3, Amount = 1000.00m, CurrentBalance = 0.00m, ApplicantName = "Ana Costa", Status = "paid" }
            );
            modelBuilder.Entity<Payment>().HasData(
                new Payment { IdPayment = 1, IdLoan = 1, DatePayment = DateTime.Now, Amount = 1000.00m },
                new Payment { IdPayment = 2, IdLoan = 3, DatePayment = DateTime.Now, Amount = 1000.00m }
            );
        }
    }
}
