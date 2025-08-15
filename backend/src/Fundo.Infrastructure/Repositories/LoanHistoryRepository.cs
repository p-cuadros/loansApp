using System.Threading.Tasks;
using Fundo.Domain.Repositories;
using Fundo.Domain.Entities;
using Fundo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Infrastructure.Repositories
{
    public class LoanHistoryRepository : ILoanHistoryRepository
    {
        private readonly LoanDbContext _db;
        public LoanHistoryRepository(LoanDbContext db) { _db = db; }

        public async Task<LoanHistory?> GetByIdAsync(int id)
        {
            return await _db.History.FirstOrDefaultAsync(l => l.IdLoan == id);
        }

        public async Task AddAsync(LoanHistory LoanHistory)
        {
            await _db.History.AddAsync(LoanHistory);
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
