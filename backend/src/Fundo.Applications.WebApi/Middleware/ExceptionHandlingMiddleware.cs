using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;

namespace Fundo.Applications.WebApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException vex)
            {
                var correlationId = context.Items[CorrelationIdMiddleware.HeaderName]?.ToString();
                _logger.LogWarning(vex, "Validation error. {Method} {Path} CorrelationId={CorrelationId}",
                    context.Request?.Method,
                    context.Request?.Path.Value,
                    correlationId);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var payload = new
                {
                    error = "validation_error",
                    message = "One or more validation errors occurred.",
                    details = vex.Errors,
                    correlationId
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
            catch (Exception ex)
            {
                var correlationId = context.Items[CorrelationIdMiddleware.HeaderName]?.ToString();
                _logger.LogError(ex, "Unhandled exception processing request. {Method} {Path} CorrelationId={CorrelationId}",
                    context.Request?.Method,
                    context.Request?.Path.Value,
                    correlationId);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var payload = new
                {
                    error = "internal_server_error",
                    message = "An unexpected error occurred.",
                    correlationId
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        }
    }
}
