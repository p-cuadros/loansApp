using Fundo.Domain.Entities;

namespace Fundo.Domain.Factories
{
    public interface ILoanFactory
    {
        Loan Create(decimal amount, string applicantName);
    }
}
