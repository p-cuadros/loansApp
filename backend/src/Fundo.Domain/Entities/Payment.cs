using System.ComponentModel.DataAnnotations;
using System;

namespace Fundo.Domain.Entities
{
    public class Payment
    {
        [Key]
        public int IdPayment { get; set; }
        public int IdLoan { get; set; }
        public virtual Loan Loan { get; set; }
        public DateTime DatePayment { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}
