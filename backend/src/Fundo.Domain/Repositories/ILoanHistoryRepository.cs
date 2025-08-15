using System.Threading.Tasks;
using Fundo.Domain.Entities;

namespace Fundo.Domain.Repositories
{
    public interface ILoanHistoryRepository
    {
        Task<LoanHistory?> GetByIdAsync(int id);
        Task AddAsync(LoanHistory LoanHistory);
        Task SaveChangesAsync();
    }
}
