using Microsoft.EntityFrameworkCore;
using Fundo.Applications.WebApi.Models;

namespace Fundo.Applications.WebApi.Data
{
    public class LoanDbContext : DbContext
    {
        public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options) { }
        public DbSet<Loan> Loans { get; set; }

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
