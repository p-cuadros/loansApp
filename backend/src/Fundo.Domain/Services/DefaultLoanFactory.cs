using Fundo.Domain.Factories;
using Fundo.Domain.Entities;

namespace Fundo.Domain.Services
{
    public class DefaultLoanFactory : ILoanFactory
    {
        public Loan Create(decimal amount, string applicantName)
        {
            return new Loan
            {
                Amount = amount,
                CurrentBalance = amount,
                ApplicantName = applicantName,
                Status = "active"
            };
        }
    }
}
