using IsBankMvc.Abstraction.Models.Payments;
using IsBankMvc.Abstraction.Types;

namespace IsBankMvc.DataAccess.Contracts
{
    public interface IPaymentRepository
    {
        Task<OperationResult<PaymentVM>> GetPayment(Guid id);
        Task<OperationResult<bool>> CreateTransaction(PreparePaymentRequest request, PreparePaymentResponse response);
    }
}
