using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Fundo.Domain.Repositories;
using Fundo.Domain.Entities;
using Fundo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly LoanDbContext _db;
        public PaymentRepository(LoanDbContext db) { _db = db; }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _db.Payments.FirstOrDefaultAsync(p => p.IdLoan == id);
        }

        public async Task<List<Payment>> GetByLoanIdAsync(int loanId)
        {
            return await _db.Payments.Where(p => p.IdLoan == loanId).ToListAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            await _db.Payments.AddAsync(payment);
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
