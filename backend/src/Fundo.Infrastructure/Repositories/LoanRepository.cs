using System.Threading.Tasks;
using Fundo.Domain.Repositories;
using Fundo.Domain.Entities;
using Fundo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Infrastructure.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly LoanDbContext _db;
        public LoanRepository(LoanDbContext db) { _db = db; }

        public async Task<Loan?> GetByIdAsync(int id)
        {
            return await _db.Loans.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task AddAsync(Loan loan)
        {
            await _db.Loans.AddAsync(loan);
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
