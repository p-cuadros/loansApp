using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Fundo.Applications.WebApi.Controllers
{
    using Fundo.Infrastructure.Data;
    using Fundo.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Fundo.Application.UseCases.Loans;
    using Fundo.Domain.Services;

    [ApiController]
    [Route("loans")]
    public class LoanManagementController : ControllerBase
    {
    private readonly GetAllLoansQueryHandler _getAllHandler;
    private readonly GetLoanByIdQueryHandler _getByIdHandler;
    private readonly GetPaymentsQueryHandler _getPaymentsHandler;
        private readonly CreateLoanHandler _createLoan;
        private readonly EditLoanHandler _editLoan;
        private readonly Fundo.Domain.Services.IPaymentService _paymentService;
        private readonly ILogger<LoanManagementController> _logger;
        public LoanManagementController(GetAllLoansQueryHandler getAllHandler, GetLoanByIdQueryHandler getByIdHandler, GetPaymentsQueryHandler getPaymentsHandler, CreateLoanHandler createLoan, EditLoanHandler editLoan, Fundo.Domain.Services.IPaymentService paymentService, ILogger<LoanManagementController> logger)
        {
            _getAllHandler = getAllHandler;
            _getByIdHandler = getByIdHandler;
            _getPaymentsHandler = getPaymentsHandler;
            _createLoan = createLoan;
            _editLoan = editLoan;
            _paymentService = paymentService;
            _logger = logger;
        }

        // GET /loans
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var loans = await _getAllHandler.Handle();
            _logger.LogInformation("Fetched {Count} loans", loans.Count);
            return Ok(loans);
        }

        // GET /loans/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var loan = await _getByIdHandler.Handle(id);
            if (loan == null)
            {
                _logger.LogWarning("Loan {Id} not found", id);
                return NotFound(new { error = "not_found", message = "Loan not found." });
            }
            return Ok(loan);
        }

        // POST /loans
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create([FromBody] CreateLoanCommand command)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var correlationId = HttpContext.Items[Middleware.CorrelationIdMiddleware.HeaderName]?.ToString();
            _logger.LogInformation("Creating loan for {Applicant} amount {Amount} CorrelationId={CorrelationId}", command.ApplicantName, command.Amount, correlationId);
            var loan = await _createLoan.Handle(command);
            _logger.LogInformation("Created loan {Id} for {Applicant} amount {Amount} CorrelationId={CorrelationId}", loan.Id, loan.ApplicantName, loan.Amount, correlationId);
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
        }

        public class PaymentRequest { public decimal amount { get; set; } }

        // POST /loans/{id}/payment
        [HttpPost("{id}/payment")]
        [Authorize]
        public async Task<ActionResult> MakePayment(int id, [FromBody] PaymentRequest request)
        {
            if (request == null) return BadRequest(new { error = "bad_request", message = "Missing body." });
            var correlationId = HttpContext.Items[Middleware.CorrelationIdMiddleware.HeaderName]?.ToString();
            _logger.LogInformation("Attempting payment for loan {Id} amount {Amount} CorrelationId={CorrelationId}", id, request.amount, correlationId);
            var result = await _paymentService.MakePaymentAsync(id, request.amount);
            if (!result.Success)
            {
                if (result.Error == "Loan not found.")
                {
                    _logger.LogWarning("Payment failed. Loan {Id} not found CorrelationId={CorrelationId}", id, correlationId);
                    return NotFound(new { error = "not_found", message = result.Error });
                }
                _logger.LogWarning("Payment failed for loan {Id}: {Reason} CorrelationId={CorrelationId}", id, result.Error, correlationId);
                return BadRequest(new { error = "payment_failed", message = result.Error });
            }
            _logger.LogInformation("Payment applied to loan {Id} CorrelationId={CorrelationId}", id, correlationId);
            return Ok(result.Data);
        }

            // GET /loans/{id}/payments
            [HttpGet("{id}/payments")]
            public async Task<ActionResult> GetPayments(int id)
            {
                var payments = await _getPaymentsHandler.Handle(id);
                if (payments == null || payments.Count == 0) return NotFound(new { error = "not_found", message = "No payments found for this loan." });
                return Ok(payments);
            }
        
        // POST /loans/{id}/payment
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> EditLoan([FromBody] EditLoanCommand command)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var correlationId = HttpContext.Items[Middleware.CorrelationIdMiddleware.HeaderName]?.ToString();
            _logger.LogInformation("Editing loan for {Applicant} amount {Amount} CorrelationId={CorrelationId}", command.ApplicantName, command.Amount, correlationId);
            var loan = await _editLoan.Handle(command);
            _logger.LogInformation("Edited loan {Id} for {Applicant} amount {Amount} CorrelationId={CorrelationId}", loan.Id, loan.ApplicantName, loan.Amount, correlationId);
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
        }
    }
}