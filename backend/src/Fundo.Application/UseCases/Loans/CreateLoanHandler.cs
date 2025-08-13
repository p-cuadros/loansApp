using Fundo.Domain.Repositories;
using Fundo.Domain.Factories;
using Fundo.Domain.Entities;
using System.Threading.Tasks;
using FluentValidation;
using Fundo.Application.UseCases.Loans.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Fundo.Application.UseCases.Loans
{
    public class CreateLoanHandler
    {
        private readonly ILoanRepository _repo;
        private readonly ILoanFactory _factory;
        private readonly IValidator<CreateLoanCommand> _validator;
    private readonly ILogger<CreateLoanHandler> _logger;
        public CreateLoanHandler(ILoanRepository repo, ILoanFactory factory, IValidator<CreateLoanCommand> validator, ILogger<CreateLoanHandler> logger)
        {
            _repo = repo;
            _factory = factory;
            _validator = validator;
            _logger = logger;
        }

        // Backwards-compatible constructor without explicit logger
        public CreateLoanHandler(ILoanRepository repo, ILoanFactory factory, IValidator<CreateLoanCommand> validator)
            : this(repo, factory, validator, NullLogger<CreateLoanHandler>.Instance)
        {
        }

        public async Task<Loan> Handle(CreateLoanCommand command)
        {
            await _validator.ValidateAndThrowAsync(command);
            _logger.LogInformation("Validated CreateLoanCommand for {Applicant} amount {Amount}", command.ApplicantName, command.Amount);
            var loan = _factory.Create(command.Amount, command.ApplicantName);
            _logger.LogInformation("Loan entity created with initial balance {Balance}", loan.CurrentBalance);
            await _repo.AddAsync(loan);
            await _repo.SaveChangesAsync();
            _logger.LogInformation("Loan persisted with Id {Id}", loan.Id);
            return loan;
        }
    }
}
