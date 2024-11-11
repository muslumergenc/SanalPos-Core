using IsBankMvc.Abstraction.Enums;
using IsBankMvc.Abstraction.Models.Payments;
using IsBankMvc.Abstraction.Types;

namespace IsBankMvc.Abstraction.Contracts.Payments
{
    public interface IPaymentProvider
    {
        ThirdPartyProvider Provider { get; set; }

        Task<OperationResult<PreparePaymentResponse>> PreparePayment(PreparePaymentRequest request);

        Task<OperationResult<BankCallbackResponse>> BankCallback(Dictionary<string, string> parameters);
        // BankCallbackParameters ParseParameters(Dictionary<string, string> parameters);
        // Task<OperationResult<bool>> VerifyPayment(Dictionary<string, string> parameters);
    }
}
