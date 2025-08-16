using System.Threading.Tasks;
using Fundo.Domain.Entities;

namespace Fundo.Domain.Repositories
{
    using System.Collections.Generic;

    public interface ILoanHistoryRepository
    {
        Task<LoanHistory?> GetByIdAsync(int id);
        Task<List<LoanHistory>> GetByLoanIdAsync(int loanId);
        Task AddAsync(LoanHistory LoanHistory);
        Task SaveChangesAsync();
    }
}
