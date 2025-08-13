using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace Fundo.Applications.WebApi.Middleware
{
    public class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = context.Request.Headers.ContainsKey(HeaderName)
                ? context.Request.Headers[HeaderName].ToString()
                : Guid.NewGuid().ToString();

            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(HeaderName))
                {
                    context.Response.Headers.Add(HeaderName, correlationId);
                }
                return Task.CompletedTask;
            });

            context.Items[HeaderName] = correlationId;

            using (LogContext.PushProperty("correlationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}
