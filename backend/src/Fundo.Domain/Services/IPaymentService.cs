using System.Threading.Tasks;

namespace Fundo.Domain.Services
{
    public interface IPaymentService
    {
        Task<PaymentResult> MakePaymentAsync(int loanId, decimal amount);
    }

    public record PaymentResult(bool Success, string? Error, object? Data)
    {
        public static PaymentResult Ok(object? data) => new(true, null, data);
        public static PaymentResult BadRequest(string error) => new(false, error, null);
        public static PaymentResult NotFound(string error) => new(false, error, null);
    }
}
