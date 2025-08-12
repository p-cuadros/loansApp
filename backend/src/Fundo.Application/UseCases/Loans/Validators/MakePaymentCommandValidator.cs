using FluentValidation;

namespace Fundo.Application.UseCases.Loans.Validators
{
    public class MakePaymentCommandValidator : AbstractValidator<MakePaymentCommand>
    {
        public MakePaymentCommandValidator()
        {
            RuleFor(x => x.LoanId)
                .GreaterThan(0);

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Invalid payment amount.");
        }
    }
}
