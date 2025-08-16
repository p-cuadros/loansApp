using System.Threading.Tasks;
using System.Collections.Generic;
using Fundo.Domain.Repositories;
using Fundo.Domain.Entities;

namespace Fundo.Application.UseCases.Loans
{
    public class GetHistoryQueryHandler
    {
        private readonly ILoanHistoryRepository _repo;
        public GetHistoryQueryHandler(ILoanHistoryRepository repo) { _repo = repo; }

        public async Task<List<LoanHistory>> Handle(int loanId)
        {
            return await _repo.GetByLoanIdAsync(loanId);
        }
    }
}
