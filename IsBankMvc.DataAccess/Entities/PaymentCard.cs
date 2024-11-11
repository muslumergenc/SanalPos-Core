using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IsBankMvc.DataAccess.Entities
{
    [Table("PaymentCard", Schema = "Payment")]
    public class PaymentCard
    {
        [Key] public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Info { get; set; } = string.Empty;
        public string? Last4Digit { get; set; }

        [ForeignKey("CustomerId")] public virtual Customer Customer { get; set; }
    }
}
