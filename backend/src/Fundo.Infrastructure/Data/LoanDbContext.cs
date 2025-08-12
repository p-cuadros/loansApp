using Fundo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Infrastructure.Data
{
    public class LoanDbContext : DbContext
    {
        public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options) { }
        public DbSet<Loan> Loans => Set<Loan>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Loan>().HasData(
                new Loan { Id = 1, Amount = 1500.00m, CurrentBalance = 500.00m, ApplicantName = "Maria Silva", Status = "active" },
                new Loan { Id = 2, Amount = 2000.00m, CurrentBalance = 2000.00m, ApplicantName = "João Souza", Status = "active" },
                new Loan { Id = 3, Amount = 1000.00m, CurrentBalance = 0.00m, ApplicantName = "Ana Costa", Status = "paid" }
            );
        }
    }
}
