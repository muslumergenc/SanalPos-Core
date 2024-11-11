using IsBankMvc.Abstraction.Contracts;
using IsBankMvc.Abstraction.Extensions;
using IsBankMvc.Abstraction.Models.Payments;
using IsBankMvc.Abstraction.Types;
using IsBankMvc.DataAccess.Contexts;
using IsBankMvc.DataAccess.Contracts;
using IsBankMvc.DataAccess.Entities;

namespace IsBankMvc.DataAccess.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ILoggerService _loggerService;
        private readonly IJsonService _jsonService;
        private readonly ApplicationDbContext _dbContext;
        public PaymentRepository(ApplicationDbContext dbContext,ILoggerService loggerService, IJsonService jsonService)
        {
            _jsonService = jsonService;
            _loggerService = loggerService;
            _dbContext = dbContext;
        }

        public async Task<OperationResult<bool>> CreateTransaction(PreparePaymentRequest request,PreparePaymentResponse response)
        {
            try
            {
                var obfuscated = request.CardNumber.ObfuscateCreditCardNumber();

                var patchedContent = _jsonService
                    .Serialize(new { Request = request, Response = response })
                    .Replace(request.CardNumber, obfuscated);

                //var transaction = new Transaction
                //{
                //    Provider = request.Provider,
                //    Currency = request.Currency,
                //    CreatedAt = DateTime.UtcNow,
                //    Amount = request.TotalAmount,
                //    PaymentId = request.OrderId,
                //    Id = request.TransactionId,
                //    RequestPayload = patchedContent,
                //    ResponsePayload = null,
                //    ConfirmedAt = null,
                //    CanceledAt = null
                //};
                //await _dbContext.Transactions.AddAsync(transaction);
                //await _dbContext.SaveChangesAsync();
                return OperationResult<bool>.Success();
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "PaymentRepository.CreateTransaction", e);
                return OperationResult<bool>.Failed();
            }
        }


        public async Task<OperationResult<PaymentVM>> GetPayment(Guid id)
        {
            try
            {
                //var payment = await _dbContext
                //    .Payments
                //    .Include(i => i.Issuer)
                //    .Include(c => c.Customer)
                //    .AsNoTracking()
                //    .SingleOrDefaultAsync(i => i.Id == id);
                //if (payment == null) return OperationResult<PaymentVM>.NotFound();
                //return OperationResult<PaymentVM>.Success(payment.ToVM());
                PaymentVM payment = new PaymentVM
                {
                    Amount = 100,
                    Provider=Abstraction.Enums.ThirdPartyProvider.IsBank,
                    Currency=Abstraction.Enums.Currency.TL,
                    Section=Abstraction.Enums.PaymentSection.UnKnown,
                };
                return OperationResult<PaymentVM>.Success(payment);
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "PaymentRepository.GetPayment", e);
                return OperationResult<PaymentVM>.Failed();
            }
        }
    }
}
