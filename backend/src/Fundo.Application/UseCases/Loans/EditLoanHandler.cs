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
    public class EditLoanHandler
    {
        private readonly ILoanRepository _repo;
        private readonly ILoanHistoryRepository _repoHistory;
        private readonly ILoanFactory _factory;
        private readonly IValidator<EditLoanCommand> _validator;
        private readonly ILogger<EditLoanHandler> _logger;
        public EditLoanHandler(ILoanRepository repo, ILoanHistoryRepository repoHistory, ILoanFactory factory, IValidator<EditLoanCommand> validator, ILogger<EditLoanHandler> logger)
        {
            _repo = repo;
            _repoHistory = repoHistory;
            _factory = factory;
            _validator = validator;
            _logger = logger;
        }

        // Backwards-compatible constructor without explicit logger
        public EditLoanHandler(ILoanRepository repo, ILoanHistoryRepository repoHistory, ILoanFactory factory, IValidator<EditLoanCommand> validator)
            : this(repo, repoHistory, factory, validator, NullLogger<EditLoanHandler>.Instance)
        {
        }

        public async Task<Loan> Handle(EditLoanCommand command)
        {
            await _validator.ValidateAndThrowAsync(command);
            var _loan = await _repo.GetByIdAsync(command.Id);
            var _history = new LoanHistory()
            {
                IdLoan = _loan.Id,
                Amount = _loan.Amount,
                CurrentBalance = _loan.CurrentBalance,
                ApplicantName = _loan.ApplicantName,
                Status = _loan.Status
            };
            await _repoHistory.AddAsync(_history);
            _logger.LogInformation("Loan history created with Amount", _loan.Amount);
            _loan.Amount = command.Amount;
            _loan.CurrentBalance = command.CurrentBalance;
            _loan.ApplicantName = command.ApplicantName;
            _loan.Status = command.Status;
            _repo.SaveChangesAsync();
            _logger.LogInformation("Loan edited with Id {Id}", _loan.Id);
            return _loan;
        }
    }
}
