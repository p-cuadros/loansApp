using System.ComponentModel.DataAnnotations;

namespace Fundo.Application.UseCases.Loans
{
    public class MakePaymentCommand
    {
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        [Range(1, int.MaxValue)]
        public int LoanId { get; set; }
    }
}
