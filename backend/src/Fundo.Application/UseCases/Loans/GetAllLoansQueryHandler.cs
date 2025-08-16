using System.Threading.Tasks;
using System.Collections.Generic;
using Fundo.Domain.Repositories;
using Fundo.Domain.Entities;

namespace Fundo.Application.UseCases.Loans
{
    public class GetAllLoansQueryHandler
    {
        private readonly ILoanRepository _repo;
        public GetAllLoansQueryHandler(ILoanRepository repo) { _repo = repo; }

        public async Task<List<Loan>> Handle()
        {
            return await _repo.ListAsync();
        }
    }
}
