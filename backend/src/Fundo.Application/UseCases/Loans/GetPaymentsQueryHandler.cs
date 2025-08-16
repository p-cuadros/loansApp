using System.Threading.Tasks;
using System.Collections.Generic;
using Fundo.Domain.Repositories;
using Fundo.Domain.Entities;

namespace Fundo.Application.UseCases.Loans
{
    public class GetPaymentsQueryHandler
    {
        private readonly IPaymentRepository _repo;
        public GetPaymentsQueryHandler(IPaymentRepository repo) { _repo = repo; }

        public async Task<List<Payment>> Handle(int loanId)
        {
            return await _repo.GetByLoanIdAsync(loanId);
        }
    }
}
