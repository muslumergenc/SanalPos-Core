using IsBankMvc.Abstraction.Enums;

namespace IsBankMvc.Abstraction.Models.Payments
{
    public class PreparePaymentResponse
    {
        public string Markup { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public PaymentGatewayMode Mode { get; set; } = PaymentGatewayMode.UnKnown;
        public ThirdPartyProvider Provider { get; set; }
    }
}
