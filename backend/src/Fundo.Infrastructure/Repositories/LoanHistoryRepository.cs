using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Fundo.Domain.Repositories;
using Fundo.Domain.Entities;
using Fundo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Fundo.Infrastructure.Repositories
{
    public class LoanHistoryRepository : ILoanHistoryRepository
    {
        private readonly LoanDbContext _db;
        public LoanHistoryRepository(LoanDbContext db) { _db = db; }

        public async Task<LoanHistory?> GetByIdAsync(int id)
        {
            return await _db.LoanHistories.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<LoanHistory>> GetByLoanIdAsync(int loanId)
        {
            return await _db.LoanHistories.Where(h => h.IdLoan == loanId).ToListAsync();
        }

        public async Task AddAsync(LoanHistory LoanHistory)
        {
            await _db.LoanHistories.AddAsync(LoanHistory);
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
