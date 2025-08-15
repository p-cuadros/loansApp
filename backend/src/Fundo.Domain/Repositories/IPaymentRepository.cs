using System.Threading.Tasks;
using Fundo.Domain.Entities;

namespace Fundo.Domain.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int id);
        Task AddAsync(Payment loan);
        Task SaveChangesAsync();
    }
}
