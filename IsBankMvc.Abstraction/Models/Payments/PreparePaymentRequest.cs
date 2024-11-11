using IsBankMvc.Abstraction.Enums;

namespace IsBankMvc.Abstraction.Models.Payments
{
    public class PreparePaymentRequest: PreparePaymentRequestBase
    {
        public Guid CustomerId { get; set; }
        public Currency Currency { get; set; }
        public decimal TotalAmount { get; set; }
        public dynamic? ExtraData { get; set; }
        public Guid TransactionId { get; set; }
        public ThirdPartyProvider Provider { get; set; }
    }
}
