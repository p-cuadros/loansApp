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
    private readonly LoanDbContext _db;
    private readonly CreateLoanHandler _createLoan;
    private readonly Fundo.Domain.Services.IPaymentService _paymentService;
    private readonly ILogger<LoanManagementController> _logger;
    public LoanManagementController(LoanDbContext db, CreateLoanHandler createLoan, Fundo.Domain.Services.IPaymentService paymentService, ILogger<LoanManagementController> logger)
        {
            _db = db;
            _createLoan = createLoan;
            _paymentService = paymentService;
            _logger = logger;
        }

        // GET /loans
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var loans = await _db.Loans.ToListAsync();
            _logger.LogInformation("Fetched {Count} loans", loans.Count);
            return Ok(loans);
        }

        // GET /loans/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var loan = await _db.Loans.FindAsync(id);
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
            var loan = await _createLoan.Handle(command);
            _logger.LogInformation("Created loan {Id} for {Applicant} amount {Amount}", loan.Id, loan.ApplicantName, loan.Amount);
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
        }

        public class PaymentRequest { public decimal amount { get; set; } }

        // POST /loans/{id}/payment
    [HttpPost("{id}/payment")]
    [Authorize]
        public async Task<ActionResult> MakePayment(int id, [FromBody] PaymentRequest request)
        {
            if (request == null) return BadRequest(new { error = "bad_request", message = "Missing body." });
            var result = await _paymentService.MakePaymentAsync(id, request.amount);
            if (!result.Success)
            {
                if (result.Error == "Loan not found.")
                {
                    _logger.LogWarning("Payment failed. Loan {Id} not found", id);
                    return NotFound(new { error = "not_found", message = result.Error });
                }
                _logger.LogWarning("Payment failed for loan {Id}: {Reason}", id, result.Error);
                return BadRequest(new { error = "payment_failed", message = result.Error });
            }
            _logger.LogInformation("Payment applied to loan {Id}", id);
            return Ok(result.Data);
        }
    }
}