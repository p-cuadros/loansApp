using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
    public LoanManagementController(LoanDbContext db, CreateLoanHandler createLoan, Fundo.Domain.Services.IPaymentService paymentService)
        {
            _db = db;
            _createLoan = createLoan;
            _paymentService = paymentService;
        }

        // GET /loans
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var loans = await _db.Loans.ToListAsync();
            return Ok(loans);
        }

        // GET /loans/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var loan = await _db.Loans.FindAsync(id);
            if (loan == null) return NotFound();
            return Ok(loan);
        }

        // POST /loans
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateLoanCommand command)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var loan = await _createLoan.Handle(command);
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
        }

        public class PaymentRequest { public decimal amount { get; set; } }

        // POST /loans/{id}/payment
        [HttpPost("{id}/payment")]
        public async Task<ActionResult> MakePayment(int id, [FromBody] PaymentRequest request)
        {
            if (request == null) return BadRequest("Missing body.");
            var result = await _paymentService.MakePaymentAsync(id, request.amount);
            if (!result.Success)
            {
                if (result.Error == "Loan not found.") return NotFound();
                return BadRequest(result.Error);
            }
            return Ok(result.Data);
        }
    }
}