using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Xunit;
using Fundo.Application.UseCases.Loans;
using Fundo.Domain.Repositories;
using Fundo.Domain.Entities;

namespace Fundo.Services.Tests.Unit
{
    public class GetQueryHandlersTests
    {
        [Fact]
        public async Task GetAllLoansQueryHandler_ReturnsListFromRepository()
        {
            var repo = new Mock<ILoanRepository>();
            var loans = new List<Loan> { new Loan { Id = 1, ApplicantName = "A", Amount = 100m, CurrentBalance = 100m, Status = "active" } };
            repo.Setup(r => r.ListAsync()).ReturnsAsync(loans);

            var handler = new GetAllLoansQueryHandler(repo.Object);
            var result = await handler.Handle();

            result.Should().BeEquivalentTo(loans);
            repo.Verify(r => r.ListAsync(), Times.Once);
        }

        [Fact]
        public async Task GetLoanByIdQueryHandler_ReturnsLoanFromRepository()
        {
            var repo = new Mock<ILoanRepository>();
            var loan = new Loan { Id = 2, ApplicantName = "B", Amount = 200m, CurrentBalance = 200m, Status = "active" };
            repo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(loan);

            var handler = new GetLoanByIdQueryHandler(repo.Object);
            var result = await handler.Handle(2);

            result.Should().BeSameAs(loan);
            repo.Verify(r => r.GetByIdAsync(2), Times.Once);
        }

        [Fact]
        public async Task GetPaymentsQueryHandler_ReturnsPaymentsFromRepository()
        {
            var repo = new Mock<IPaymentRepository>();
            var payments = new List<Payment> { new Payment { IdPayment = 1, IdLoan = 1, Amount = 50m } };
            repo.Setup(r => r.GetByLoanIdAsync(1)).ReturnsAsync(payments);

            var handler = new GetPaymentsQueryHandler(repo.Object);
            var result = await handler.Handle(1);

            result.Should().BeEquivalentTo(payments);
            repo.Verify(r => r.GetByLoanIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetHistoryQueryHandler_ReturnsHistoryFromRepository()
        {
            var repo = new Mock<ILoanHistoryRepository>();
            var history = new List<LoanHistory> { new LoanHistory { Id = 1, IdLoan = 1, Amount = 100m, CurrentBalance = 100m, ApplicantName = "A", Status = "active" } };
            repo.Setup(r => r.GetByLoanIdAsync(1)).ReturnsAsync(history);

            var handler = new GetHistoryQueryHandler(repo.Object);
            var result = await handler.Handle(1);

            result.Should().BeEquivalentTo(history);
            repo.Verify(r => r.GetByLoanIdAsync(1), Times.Once);
        }
    }
}
