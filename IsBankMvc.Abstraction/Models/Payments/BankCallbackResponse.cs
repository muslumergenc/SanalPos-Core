using IsBankMvc.Abstraction.Enums;

namespace IsBankMvc.Abstraction.Models.Payments
{
    public class BankCallbackResponse
    {
        public bool Success { get; set; }
        public Guid PaymentId { get; set; }
        public ThirdPartyProvider Provider { get; set; } = ThirdPartyProvider.PayZee;
        public BankCallbackParameters Parameters { get; set; } = new();
    }
}
