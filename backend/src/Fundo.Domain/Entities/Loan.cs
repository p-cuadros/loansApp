using System.ComponentModel.DataAnnotations;

namespace Fundo.Domain.Entities
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal CurrentBalance { get; set; }
        [Required]
        [MaxLength(200)]
        public string ApplicantName { get; set; } = string.Empty;
        [RegularExpression("^(active|paid)$")]
        public string Status { get; set; } = "active"; // "active" or "paid"
    }
}
