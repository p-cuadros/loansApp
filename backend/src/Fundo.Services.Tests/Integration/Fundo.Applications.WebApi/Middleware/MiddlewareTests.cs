using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace Fundo.Services.Tests.Integration
{
    public class MiddlewareTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        public MiddlewareTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CorrelationIdMiddleware_AddsHeader()
        {
            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/loans");
            Assert.True(resp.Headers.Contains("X-Correlation-Id"));
        }

        [Fact]
        public async Task ExceptionHandlingMiddleware_Returns500_OnUnhandled()
        {
            using var app = _factory.WithWebHostBuilder(builder =>
            {
                builder.Configure(app =>
                {
                    app.UseMiddleware<Fundo.Applications.WebApi.Middleware.CorrelationIdMiddleware>();
                    app.UseMiddleware<Fundo.Applications.WebApi.Middleware.ExceptionHandlingMiddleware>();
                    app.Run(ctx => throw new System.Exception("boom"));
                });
            });
            var client = app.CreateClient();
            var resp = await client.GetAsync("/");
            Assert.Equal(HttpStatusCode.InternalServerError, resp.StatusCode);
            var json = await resp.Content.ReadAsStringAsync();
            Assert.Contains("internal_server_error", json);
        }
    }
}
