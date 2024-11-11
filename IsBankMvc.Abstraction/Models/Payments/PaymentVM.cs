using IsBankMvc.Abstraction.Enums;

namespace IsBankMvc.Abstraction.Models.Payments
{
    public class PaymentVM
    {
        public Guid Id { get; set; }
        public int Index { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CanceledAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public ThirdPartyProvider Provider { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public string? TransactionId { get; set; }
        public string? Response { get; set; }
        public string? Note { get; set; }
        public Guid CreatedBy { get; set; }
        public PaymentSection Section { get; set; }

        public string Issuer { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;

        public bool IsAlreadyLabeled()
        {
            // return ConfirmedAt.HasValue || CanceledAt.HasValue;
            return ConfirmedAt.HasValue;
        }
    }

    public class TransactionVM
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public ThirdPartyProvider Provider { get; set; }
        public DateTime? CanceledAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RequestPayload { get; set; } = string.Empty;
        public string? ResponsePayload { get; set; }
    }
}
