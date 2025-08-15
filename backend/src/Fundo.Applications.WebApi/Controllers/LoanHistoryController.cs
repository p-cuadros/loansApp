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
    [Route("payments")]
    public class LoanHistoryController : ControllerBase
    {
    private readonly LoanDbContext _db;
    private readonly ILogger<LoanManagementController> _logger;
    public LoanHistoryController(LoanDbContext db, ILogger<LoanManagementController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET /loans/{id}/payments
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var payments = await _db.Payments.FindAsync(id);
            if (payments == null)
            {
                _logger.LogWarning("Loan {Id} not found", id);
                return NotFound(new { error = "not_found", message = "Loan not found." });
            }
            return Ok(payments);
        }
   }
}