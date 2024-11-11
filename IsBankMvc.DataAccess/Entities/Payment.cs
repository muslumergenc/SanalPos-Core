using IsBankMvc.Abstraction.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using IsBankMvc.Abstraction.Models.Payments;

namespace IsBankMvc.DataAccess.Entities
{
    public class Payment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Index { get; set; }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CanceledAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public ThirdPartyProvider Provider { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }

        public PaymentSection Section { get; set; }
        public Guid? ApprovedBy { get; set; }
        public Guid? CanceledBy { get; set; }
        [MaxLength(64)] public string? TransactionId { get; set; } = string.Empty;
        [MaxLength(4096)] public string? Response { get; set; }
        [MaxLength(1024)] public string? Note { get; set; } = string.Empty;

        [ForeignKey("CustomerId")] public virtual Customer Customer { get; set; }
        [ForeignKey("ApprovedBy")] public virtual User? Approver { get; set; }
        [ForeignKey("CanceledBy")] public virtual User? Canceller { get; set; }
        [ForeignKey("CreatedBy")] public virtual User Issuer { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
        public PaymentVM ToVM()
        {
            return new PaymentVM
            {
                Id = Id,
                CreatedAt = CreatedAt,
                Amount = Amount,
                Currency = Currency,
                Index = Index,
                Provider = Provider,
                Response = Response,
                CanceledAt = CanceledAt,
                ConfirmedAt = ConfirmedAt,
                ExpiresAt = ExpiresAt,
                TransactionId = TransactionId,
                CustomerId = CustomerId,
                CreatedBy = CreatedBy,
                Note = Note,
                Section = Section,
                Issuer = $"{Issuer?.Name} {Issuer?.Surname}".Trim(),
                Customer = $"{Customer?.Name} {Customer?.Surname}".Trim()
            };
        }

        public bool IsAlreadyLabeled()
        {
            // return ConfirmedAt.HasValue || CanceledAt.HasValue;
            return ConfirmedAt.HasValue;
        }
    }
}
