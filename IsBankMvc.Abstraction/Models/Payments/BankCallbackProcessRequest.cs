using IsBankMvc.Abstraction.Enums;

namespace IsBankMvc.Abstraction.Models.Payments
{
    public class BankCallbackProcessRequest
    {
        public bool Success { get; set; }
        public Guid PaymentId { get; set; }
        public ThirdPartyProvider Provider { get; set; } = ThirdPartyProvider.PayZee;

        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}
