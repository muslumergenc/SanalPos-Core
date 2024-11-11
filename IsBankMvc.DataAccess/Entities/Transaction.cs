using IsBankMvc.Abstraction.Enums;
using IsBankMvc.Abstraction.Models.Payments;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IsBankMvc.DataAccess.Entities
{
    public class Transaction
    {
        [Key] public Guid Id { get; set; }
        public Guid PaymentId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CanceledAt { get; set; }

        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public ThirdPartyProvider Provider { get; set; }

        public string RequestPayload { get; set; } = string.Empty;
        public string? ResponsePayload { get; set; }

        [ForeignKey("PaymentId")] public virtual Payment Payment { get; set; }


        public TransactionVM ToVM()
        {
            return new TransactionVM
            {
                Id = Id,
                PaymentId = PaymentId,
                Amount = Amount,
                Currency = Currency,
                Provider = Provider,
                CanceledAt = CanceledAt,
                ConfirmedAt = ConfirmedAt,
                CreatedAt = CreatedAt,
                RequestPayload = RequestPayload,
                ResponsePayload = ResponsePayload
            };
        }
    }
}
