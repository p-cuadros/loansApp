using System.Threading.Tasks;
using System.Collections.Generic;
using Fundo.Domain.Entities;

namespace Fundo.Domain.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int id);
        Task<List<Payment>> GetByLoanIdAsync(int loanId);
        Task AddAsync(Payment loan);
        Task SaveChangesAsync();
    }
}
