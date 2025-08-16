using System.Threading.Tasks;
using Fundo.Domain.Repositories;
using Fundo.Domain.Entities;

namespace Fundo.Application.UseCases.Loans
{
    public class GetLoanByIdQueryHandler
    {
        private readonly ILoanRepository _repo;
        public GetLoanByIdQueryHandler(ILoanRepository repo) { _repo = repo; }

        public async Task<Loan?> Handle(int id)
        {
            return await _repo.GetByIdAsync(id);
        }
    }
}
