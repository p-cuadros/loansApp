using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Integration
{
    public class LoanManagementControllerTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public LoanManagementControllerTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetAllLoans_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/loans");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetLoanById_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/loans/1");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateLoan_ShouldReturnCreated()
        {
            var loanJson = "{\"amount\":1000,\"applicantName\":\"Test User\"}";
            var content = new StringContent(loanJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/loans", content);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task MakePayment_ShouldReturnOk()
        {
            var paymentContent = new StringContent("{\"amount\":100}", System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/loans/1/payment", paymentContent);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
