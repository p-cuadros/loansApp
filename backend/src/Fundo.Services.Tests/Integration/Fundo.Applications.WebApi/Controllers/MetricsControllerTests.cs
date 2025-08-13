using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Fundo.Services.Tests.Integration
{
    public class MetricsControllerTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public MetricsControllerTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Metrics_ShouldAcceptPayload()
        {
            var payload = new StringContent("{\"method\":\"GET\",\"url\":\"/loans\",\"status\":200,\"durationMs\":5}", Encoding.UTF8, "application/json");
            var resp = await _client.PostAsync("/metrics/http-client", payload);
            Assert.Equal(HttpStatusCode.Accepted, resp.StatusCode);
        }

        [Fact]
        public async Task Metrics_ShouldReturnBadRequest_WhenMissingBody()
        {
            var resp = await _client.PostAsync("/metrics/http-client", new StringContent("null", Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }
    }
}
