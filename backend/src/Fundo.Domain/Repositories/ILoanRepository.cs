using System.Threading.Tasks;
using System.Collections.Generic;
using Fundo.Domain.Entities;

namespace Fundo.Domain.Repositories
{
    public interface ILoanRepository
    {
        Task<Loan?> GetByIdAsync(int id);
        Task<List<Loan>> ListAsync();
        Task AddAsync(Loan loan);
        Task SaveChangesAsync();
    }
}
