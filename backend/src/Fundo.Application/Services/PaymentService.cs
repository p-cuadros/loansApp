using System.Threading.Tasks;
using Fundo.Domain.Repositories;
using FluentValidation;
using Fundo.Application.UseCases.Loans;
using Fundo.Application.UseCases.Loans.Validators;
using Fundo.Domain.Services;
using Fundo.Domain.Entities;
using System;

namespace Fundo.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ILoanRepository _repo;
        private readonly IPaymentRepository _repoPayments;
        private readonly IValidator<MakePaymentCommand> _validator;
        public PaymentService(ILoanRepository repo, IPaymentRepository repoPayments, IValidator<MakePaymentCommand> validator)
        {
            _repo = repo;
            _repoPayments = repoPayments;
            _validator = validator;
        }

        public async Task<PaymentResult> MakePaymentAsync(int loanId, decimal amount)
        {
            await _validator.ValidateAndThrowAsync(new MakePaymentCommand { LoanId = loanId, Amount = amount });
            var loan = await _repo.GetByIdAsync(loanId);
            if (loan == null) return PaymentResult.NotFound("Loan not found.");
            if (loan.Status == "paid") return PaymentResult.BadRequest("Loan already paid.");
            if (amount <= 0) return PaymentResult.BadRequest("Invalid payment amount.");
            if (amount > loan.CurrentBalance) return PaymentResult.BadRequest("Payment exceeds current balance.");

            loan.CurrentBalance -= amount;
            if (loan.CurrentBalance == 0) loan.Status = "paid";
            var _payment = new Payment()
            {
                IdLoan = loanId,
                DatePayment = DateTime.Now,
                Amount = amount,
            };
            await _repoPayments.AddAsync(_payment);
            await _repo.SaveChangesAsync();
            return PaymentResult.Ok(loan);
        }
    }
}
