using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Fundo.Application.Services;
using Fundo.Application.UseCases.Loans;
using Fundo.Application.UseCases.Loans.Validators;
using Fundo.Domain.Entities;
using Fundo.Domain.Repositories;
using Moq;
using Xunit;

namespace Fundo.Services.Tests.Unit
{
    public class PaymentServiceTests
    {
        [Fact]
        public async Task MakePaymentAsync_ShouldDecreaseBalance_WhenValid()
        {
            var repo = new Mock<ILoanRepository>();
            var loan = new Loan { Id = 1, Amount = 1000m, CurrentBalance = 500m, ApplicantName = "A", Status = "active" };
            repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(loan);
            repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            IValidator<MakePaymentCommand> validator = new MakePaymentCommandValidator();
            var svc = new PaymentService(repo.Object, validator);

            var result = await svc.MakePaymentAsync(1, 200m);

            result.Success.Should().BeTrue();
            loan.CurrentBalance.Should().Be(300m);
            repo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task MakePaymentAsync_ShouldReturnBadRequest_WhenOverpay()
        {
            var repo = new Mock<ILoanRepository>();
            var loan = new Loan { Id = 1, Amount = 1000m, CurrentBalance = 500m, ApplicantName = "A", Status = "active" };
            repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(loan);
            IValidator<MakePaymentCommand> validator = new MakePaymentCommandValidator();
            var svc = new PaymentService(repo.Object, validator);

            var result = await svc.MakePaymentAsync(1, 600m);

            result.Success.Should().BeFalse();
            result.Error.Should().Contain("exceeds");
        }

        [Fact]
        public async Task MakePaymentAsync_ShouldReturnNotFound_WhenLoanMissing()
        {
            var repo = new Mock<ILoanRepository>();
            repo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Loan)null);
            IValidator<MakePaymentCommand> validator = new MakePaymentCommandValidator();
            var svc = new PaymentService(repo.Object, validator);

            var result = await svc.MakePaymentAsync(2, 100m);

            result.Success.Should().BeFalse();
            result.Error.Should().Contain("not found");
        }

        [Fact]
        public async Task MakePaymentAsync_ShouldReturnBadRequest_WhenAlreadyPaid()
        {
            var repo = new Mock<ILoanRepository>();
            var loan = new Loan { Id = 3, Amount = 1000m, CurrentBalance = 0m, ApplicantName = "A", Status = "paid" };
            repo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(loan);
            IValidator<MakePaymentCommand> validator = new MakePaymentCommandValidator();
            var svc = new PaymentService(repo.Object, validator);

            var result = await svc.MakePaymentAsync(3, 10m);

            result.Success.Should().BeFalse();
            result.Error.Should().Contain("already paid");
        }

        [Fact]
        public async Task MakePaymentAsync_ShouldThrowValidation_WhenInvalidAmount()
        {
            var repo = new Mock<ILoanRepository>();
            var loan = new Loan { Id = 4, Amount = 1000m, CurrentBalance = 1000m, ApplicantName = "A", Status = "active" };
            repo.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(loan);
            IValidator<MakePaymentCommand> validator = new MakePaymentCommandValidator();
            var svc = new PaymentService(repo.Object, validator);

            var act = async () => await svc.MakePaymentAsync(4, 0m);
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }
    }
}
