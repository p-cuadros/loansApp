using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Fundo.Services.Tests.Integration
{
    public class LoanAuthAndErrorsTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public LoanAuthAndErrorsTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        }

        [Fact]
        public async Task CreateLoan_ShouldReturn401_WithoutToken()
        {
            var content = new StringContent("{\"amount\":100,\"applicantName\":\"A\"}", Encoding.UTF8, "application/json");
            var resp = await _client.PostAsync("/loans", content);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task MakePayment_ShouldReturn400_WhenOverpay()
        {
            var login = new StringContent("{\"username\":\"admin\",\"password\":\"admin\"}", Encoding.UTF8, "application/json");
            var loginResp = await _client.PostAsync("/auth/login", login);
            loginResp.EnsureSuccessStatusCode();
            var token = System.Text.Json.JsonDocument.Parse(await loginResp.Content.ReadAsStringAsync()).RootElement.GetProperty("token").GetString();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var paymentContent = new StringContent("{\"amount\":999999}", Encoding.UTF8, "application/json");
            var resp = await _client.PostAsync("/loans/1/payment", paymentContent);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }
    }
}
