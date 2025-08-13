using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Fundo.Services.Tests.Integration
{
    public class AuthControllerTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public AuthControllerTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenMissingBody()
        {
            var resp = await _client.PostAsync("/auth/login", new StringContent("null", Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenInvalidCredentials()
        {
            var body = new StringContent("{\"username\":\"x\",\"password\":\"y\"}", Encoding.UTF8, "application/json");
            var resp = await _client.PostAsync("/auth/login", body);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenValid()
        {
            var body = new StringContent("{\"username\":\"admin\",\"password\":\"admin\"}", Encoding.UTF8, "application/json");
            var resp = await _client.PostAsync("/auth/login", body);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            Assert.Contains("token", json);
        }
    }
}
