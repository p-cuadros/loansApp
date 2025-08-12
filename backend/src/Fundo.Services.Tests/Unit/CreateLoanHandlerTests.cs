using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Fundo.Application.UseCases.Loans;
using Fundo.Application.UseCases.Loans.Validators;
using Fundo.Domain.Entities;
using Fundo.Domain.Factories;
using Fundo.Domain.Repositories;
using Moq;
using Xunit;

namespace Fundo.Services.Tests.Unit
{
    public class CreateLoanHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCreateLoan_WhenValid()
        {
            var repo = new Mock<ILoanRepository>();
            repo.Setup(r => r.AddAsync(It.IsAny<Loan>())).Returns(Task.CompletedTask);
            repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var factory = new Mock<ILoanFactory>();
            factory.Setup(f => f.Create(1000m, "User")).Returns(new Loan { Amount = 1000m, CurrentBalance = 1000m, ApplicantName = "User", Status = "active" });
            IValidator<CreateLoanCommand> validator = new CreateLoanCommandValidator();
            var handler = new CreateLoanHandler(repo.Object, factory.Object, validator);

            var result = await handler.Handle(new CreateLoanCommand { Amount = 1000m, ApplicantName = "User" });

            result.Should().NotBeNull();
            result.Amount.Should().Be(1000m);
            result.CurrentBalance.Should().Be(1000m);
            result.ApplicantName.Should().Be("User");
            repo.Verify(r => r.AddAsync(It.IsAny<Loan>()), Times.Once);
            repo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenInvalid()
        {
            var repo = new Mock<ILoanRepository>();
            var factory = new Mock<ILoanFactory>();
            IValidator<CreateLoanCommand> validator = new CreateLoanCommandValidator();
            var handler = new CreateLoanHandler(repo.Object, factory.Object, validator);

            var act = async () => await handler.Handle(new CreateLoanCommand { Amount = 0, ApplicantName = "" });
            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
