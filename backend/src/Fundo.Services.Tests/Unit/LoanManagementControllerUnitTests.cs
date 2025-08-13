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
            var db = InMemoryDb();
            var create = new CreateLoanHandler(new Mock<Fundo.Domain.Repositories.ILoanRepository>().Object,
                                               new Mock<Fundo.Domain.Factories.ILoanFactory>().Object,
                                               new CreateLoanCommandValidator());
            var paySvc = new Mock<IPaymentService>();
            var controller = new LoanManagementController(db, create, paySvc.Object, NullLogger<LoanManagementController>.Instance)
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
            var db = InMemoryDb();
            var create = new CreateLoanHandler(new Mock<Fundo.Domain.Repositories.ILoanRepository>().Object,
                                               new Mock<Fundo.Domain.Factories.ILoanFactory>().Object,
                                               new CreateLoanCommandValidator());
            var paySvc = new Mock<IPaymentService>();
            var controller = new LoanManagementController(db, create, paySvc.Object, NullLogger<LoanManagementController>.Instance)
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
            var db = InMemoryDb();
            var create = new CreateLoanHandler(new Mock<Fundo.Domain.Repositories.ILoanRepository>().Object,
                                               new Mock<Fundo.Domain.Factories.ILoanFactory>().Object,
                                               new CreateLoanCommandValidator());
            var paySvc = new Mock<IPaymentService>();
            paySvc.Setup(s => s.MakePaymentAsync(1, 10m)).ReturnsAsync(Fundo.Domain.Services.PaymentResult.NotFound("Loan not found."));
            var controller = new LoanManagementController(db, create, paySvc.Object, NullLogger<LoanManagementController>.Instance)
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
