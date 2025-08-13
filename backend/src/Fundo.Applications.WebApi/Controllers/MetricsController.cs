using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("metrics")] 
    public class MetricsController : ControllerBase
    {
        private readonly ILogger<MetricsController> _logger;
        public MetricsController(ILogger<MetricsController> logger)
        {
            _logger = logger;
        }

        public record HttpClientMetric(string method, string url, int status, long durationMs, string? correlationId);

        [HttpPost("http-client")]
        [AllowAnonymous]
        public IActionResult HttpClient([FromBody] HttpClientMetric? metric)
        {
            if (metric is null)
            {
                return BadRequest(new { error = "bad_request", message = "Missing or invalid body" });
            }
            var correlationId = metric.correlationId ?? HttpContext.Items[Middleware.CorrelationIdMiddleware.HeaderName]?.ToString();
            // Log structured event with app=loan-frontend semantics
            using (_logger.BeginScope(new System.Collections.Generic.Dictionary<string, object>
            {
                ["app"] = "loan-frontend",
                ["correlationId"] = correlationId ?? string.Empty
            }))
            {
                _logger.LogInformation("http_client metric {Method} {Url} {Status} {DurationMs} CorrelationId={CorrelationId}",
                    metric.method ?? "UNKNOWN", metric.url ?? "UNKNOWN", metric.status, metric.durationMs, correlationId ?? string.Empty);
            }
            return Accepted();
        }
    }
}
