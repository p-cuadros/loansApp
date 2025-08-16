using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Fundo.Applications.WebApi.Controllers;
using Fundo.Infrastructure.Data;
using Fundo.Application.UseCases.Loans;
// ...existing using directives...
using Fundo.Domain.Services;
using FluentValidation;
using Fundo.Application.UseCases.Loans.Validators;
using Fundo.Domain.Entities;

namespace Fundo.Services.Tests.Unit
{
    public class LoanManagementControllerUnitTests
    {
        private LoanDbContext InMemoryDb()
        {
            var opts = new DbContextOptionsBuilder<LoanDbContext>().UseInMemoryDatabase("unit_db").Options;
            var db = new LoanDbContext(opts);
            db.Database.EnsureCreated();
            return db;
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenMissing()
        {
            var loanRepoMock = new Mock<Fundo.Domain.Repositories.ILoanRepository>();
            var loanHistoryRepoMock = new Mock<Fundo.Domain.Repositories.ILoanHistoryRepository>();
            var paymentRepoMock = new Mock<Fundo.Domain.Repositories.IPaymentRepository>();

            var getAll = new GetAllLoansQueryHandler(loanRepoMock.Object);
            var getById = new GetLoanByIdQueryHandler(loanRepoMock.Object);
            var getPayments = new GetPaymentsQueryHandler(paymentRepoMock.Object);

            var create = new CreateLoanHandler(loanRepoMock.Object,
                                               new Mock<Fundo.Domain.Factories.ILoanFactory>().Object,
                                               new CreateLoanCommandValidator());
            var edit = new EditLoanHandler(loanRepoMock.Object,
                                           loanHistoryRepoMock.Object,
                                           new Mock<Fundo.Domain.Factories.ILoanFactory>().Object,
                                           new EditLoanCommandValidator(),
                                           NullLogger<EditLoanHandler>.Instance);
            var paySvc = new Mock<IPaymentService>();
            var controller = new LoanManagementController(getAll, getById, getPayments, create, edit, paySvc.Object, NullLogger<LoanManagementController>.Instance)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
                }
            };

            var resp = await controller.GetById(999);
            resp.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task MakePayment_ShouldReturnBadRequest_WhenNullBody()
        {
            var loanRepoMock2 = new Mock<Fundo.Domain.Repositories.ILoanRepository>();
            var loanHistoryRepoMock2 = new Mock<Fundo.Domain.Repositories.ILoanHistoryRepository>();
            var paymentRepoMock2 = new Mock<Fundo.Domain.Repositories.IPaymentRepository>();

            var getAll2 = new GetAllLoansQueryHandler(loanRepoMock2.Object);
            var getById2 = new GetLoanByIdQueryHandler(loanRepoMock2.Object);
            var getPayments2 = new GetPaymentsQueryHandler(paymentRepoMock2.Object);

            var create2 = new CreateLoanHandler(loanRepoMock2.Object,
                                               new Mock<Fundo.Domain.Factories.ILoanFactory>().Object,
                                               new CreateLoanCommandValidator());
            var paySvc2 = new Mock<IPaymentService>();
            var edit2 = new EditLoanHandler(loanRepoMock2.Object,
                                           loanHistoryRepoMock2.Object,
                                           new Mock<Fundo.Domain.Factories.ILoanFactory>().Object,
                                           new EditLoanCommandValidator(),
                                           NullLogger<EditLoanHandler>.Instance);
            var controller = new LoanManagementController(getAll2, getById2, getPayments2, create2, edit2, paySvc2.Object, NullLogger<LoanManagementController>.Instance)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
                }
            };

            var resp = await controller.MakePayment(1, null);
            resp.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task MakePayment_ShouldReturnNotFound_WhenServiceNotFound()
        {
            var loanRepoMock3 = new Mock<Fundo.Domain.Repositories.ILoanRepository>();
            var loanHistoryRepoMock3 = new Mock<Fundo.Domain.Repositories.ILoanHistoryRepository>();
            var paymentRepoMock3 = new Mock<Fundo.Domain.Repositories.IPaymentRepository>();

            var getAll3 = new GetAllLoansQueryHandler(loanRepoMock3.Object);
            var getById3 = new GetLoanByIdQueryHandler(loanRepoMock3.Object);
            var getPayments3 = new GetPaymentsQueryHandler(paymentRepoMock3.Object);

            var create3 = new CreateLoanHandler(loanRepoMock3.Object,
                                               new Mock<Fundo.Domain.Factories.ILoanFactory>().Object,
                                               new CreateLoanCommandValidator());
            var paySvc3 = new Mock<IPaymentService>();
            var edit3 = new EditLoanHandler(loanRepoMock3.Object,
                                           loanHistoryRepoMock3.Object,
                                           new Mock<Fundo.Domain.Factories.ILoanFactory>().Object,
                                           new EditLoanCommandValidator(),
                                           NullLogger<EditLoanHandler>.Instance);
            paySvc3.Setup(s => s.MakePaymentAsync(1, 10m)).ReturnsAsync(Fundo.Domain.Services.PaymentResult.NotFound("Loan not found."));
            var controller = new LoanManagementController(getAll3, getById3, getPayments3, create3, edit3, paySvc3.Object, NullLogger<LoanManagementController>.Instance)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
                }
            };

            var resp = await controller.MakePayment(1, new LoanManagementController.PaymentRequest { amount = 10m });
            resp.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
