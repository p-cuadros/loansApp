using FluentValidation;

namespace Fundo.Application.UseCases.Loans.Validators
{
    public class EditLoanCommandValidator : AbstractValidator<EditLoanCommand>
    {
        public EditLoanCommandValidator()
        {
            RuleFor(x => x.IdLoan)
                .GreaterThan(0).WithMessage("IdLoan must be greater than 0.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.ApplicantName)
                .NotEmpty().WithMessage("ApplicantName is required.")
                .MaximumLength(200);
        }
    }
}
