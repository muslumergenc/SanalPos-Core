using IsBankMvc.Abstraction.Models.Payments;
using IsBankMvc.Abstraction.Types;

namespace IsBankMvc.Abstraction.Interfaces.Payments
{
    public interface IPaymentService
    {
        Task<OperationResult<PreparePaymentResponse>> PreparePayment(PreparePaymentRequestBase request);
        Task<BankCallbackProcessResponse> BankCallback(BankCallbackProcessRequest process);
    }
}
