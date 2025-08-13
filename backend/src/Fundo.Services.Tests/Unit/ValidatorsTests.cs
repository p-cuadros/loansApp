using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using Fundo.Application.UseCases.Loans;
using Fundo.Application.UseCases.Loans.Validators;
using Xunit;

namespace Fundo.Services.Tests.Unit
{
    public class ValidatorsTests
    {
        [Fact]
        public void CreateLoanCommandValidator_ShouldValidate()
        {
            var validator = new CreateLoanCommandValidator();
            var good = new CreateLoanCommand { Amount = 10, ApplicantName = "Joe" };
            var bad = new CreateLoanCommand { Amount = 0, ApplicantName = "" };

            var goodResult = validator.TestValidate(good);
            goodResult.ShouldNotHaveValidationErrorFor(x => x.Amount);
            goodResult.ShouldNotHaveValidationErrorFor(x => x.ApplicantName);

            var badResult = validator.TestValidate(bad);
            badResult.ShouldHaveValidationErrorFor(x => x.Amount);
            badResult.ShouldHaveValidationErrorFor(x => x.ApplicantName);
        }

        [Fact]
        public void MakePaymentCommandValidator_ShouldValidate()
        {
            var validator = new MakePaymentCommandValidator();
            var good = new MakePaymentCommand { LoanId = 1, Amount = 10 };
            var bad = new MakePaymentCommand { LoanId = 0, Amount = 0 };

            var goodResult = validator.TestValidate(good);
            goodResult.ShouldNotHaveValidationErrorFor(x => x.LoanId);
            goodResult.ShouldNotHaveValidationErrorFor(x => x.Amount);

            var badResult = validator.TestValidate(bad);
            badResult.ShouldHaveValidationErrorFor(x => x.LoanId);
            badResult.ShouldHaveValidationErrorFor(x => x.Amount);
        }
    }
}
