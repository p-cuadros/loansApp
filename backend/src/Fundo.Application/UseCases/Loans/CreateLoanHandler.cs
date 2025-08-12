using Fundo.Domain.Repositories;
using Fundo.Domain.Factories;
using Fundo.Domain.Entities;
using System.Threading.Tasks;
using FluentValidation;
using Fundo.Application.UseCases.Loans.Validators;

namespace Fundo.Application.UseCases.Loans
{
    public class CreateLoanHandler
    {
        private readonly ILoanRepository _repo;
        private readonly ILoanFactory _factory;
        private readonly IValidator<CreateLoanCommand> _validator;
        public CreateLoanHandler(ILoanRepository repo, ILoanFactory factory, IValidator<CreateLoanCommand> validator)
        {
            _repo = repo;
            _factory = factory;
            _validator = validator;
        }

        public async Task<Loan> Handle(CreateLoanCommand command)
        {
            await _validator.ValidateAndThrowAsync(command);
            var loan = _factory.Create(command.Amount, command.ApplicantName);
            await _repo.AddAsync(loan);
            await _repo.SaveChangesAsync();
            return loan;
        }
    }
}
