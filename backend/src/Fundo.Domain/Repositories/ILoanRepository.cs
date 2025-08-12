using System.Threading.Tasks;
using Fundo.Domain.Entities;

namespace Fundo.Domain.Repositories
{
    public interface ILoanRepository
    {
        Task<Loan?> GetByIdAsync(int id);
        Task AddAsync(Loan loan);
        Task SaveChangesAsync();
    }
}
