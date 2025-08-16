using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Fundo.Applications.WebApi.Controllers
{
    using Fundo.Infrastructure.Data;
    using Fundo.Domain.Entities;
    using Fundo.Domain.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Fundo.Application.UseCases.Loans;
    using Fundo.Domain.Services;

    [ApiController]
    [Route("loanhistory")]
    public class LoanHistoryController : ControllerBase
    {
    private readonly GetHistoryQueryHandler _getHistoryHandler;
    private readonly ILogger<LoanHistoryController> _logger;
    public LoanHistoryController(GetHistoryQueryHandler getHistoryHandler, ILogger<LoanHistoryController> logger)
        {
            _getHistoryHandler = getHistoryHandler;
            _logger = logger;
        }

        // GET /loanhistory/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByLoanId(int id)
        {
            var history = await _getHistoryHandler.Handle(id);
            if (history == null || history.Count == 0)
            {
                _logger.LogWarning("No history found for loan {Id}", id);
                return NotFound(new { error = "not_found", message = "No history found for this loan." });
            }
            return Ok(history);
        }
    }
}