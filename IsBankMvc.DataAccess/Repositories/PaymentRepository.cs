using IsBankMvc.Abstraction.Contracts;
using IsBankMvc.Abstraction.Enums;
using IsBankMvc.Abstraction.Extensions;
using IsBankMvc.Abstraction.Models.Payments;
using IsBankMvc.Abstraction.Models.User;
using IsBankMvc.Abstraction.Types;
using IsBankMvc.DataAccess.Contexts;
using IsBankMvc.DataAccess.Contracts;
using IsBankMvc.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IsBankMvc.DataAccess.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ILoggerService _loggerService;
        private readonly IJsonService _jsonService;
        private readonly ApplicationDbContext _dbContext;
        public PaymentRepository(ApplicationDbContext dbContext, ILoggerService loggerService, IJsonService jsonService)
        {
            _jsonService = jsonService;
            _loggerService = loggerService;
            _dbContext = dbContext;
        }
        public async Task<OperationResult<bool>> CreateTransaction(PreparePaymentRequest request, PreparePaymentResponse response)
        {
            try
            {
                var obfuscated = request.CardNumber.ObfuscateCreditCardNumber();

                var patchedContent = _jsonService
                    .Serialize(new { Request = request, Response = response })
                    .Replace(request.CardNumber, obfuscated);

                var existingTransaction = _dbContext.Transactions.Find(request.TransactionId);
                if (existingTransaction == null)
                {
                    var transaction = new Transaction
                    {
                        Provider = request.Provider,
                        Currency = request.Currency,
                        CreatedAt = DateTime.UtcNow,
                        Amount = request.TotalAmount,
                        PaymentId = request.OrderId,
                        Id = request.TransactionId,
                        RequestPayload = patchedContent,
                        ResponsePayload = null,
                        ConfirmedAt = null,
                        CanceledAt = null
                    };
                    //_dbContext.Transactions.Add(transaction);
                    //_dbContext.SaveChangesAsync();
                }
                await _loggerService.Info("PaymentRepository.CreateTransaction", "PaymentRepository");
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
                    Provider = Abstraction.Enums.ThirdPartyProvider.IsBank,
                    Currency = Abstraction.Enums.Currency.TL,
                    Section = Abstraction.Enums.PaymentSection.UnKnown,
                    Customer ="test"
                };
                return OperationResult<PaymentVM>.Success(payment);
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "PaymentRepository.GetPayment", e);
                return OperationResult<PaymentVM>.Failed();
            }
        }
        public async Task<OperationResult<bool>> UpdateTransaction(Guid transactionId, BankCallbackResponse response)
        {
            try
            {
                var transaction = await _dbContext.Transactions
                    .Include(i => i.Payment)
                    .SingleOrDefaultAsync(i => i.Id == transactionId);

                if (transaction == null) return OperationResult<bool>.NotFound();

                if (transaction.ConfirmedAt.HasValue || transaction.CanceledAt.HasValue)
                    return OperationResult<bool>.Rejected();

                transaction.ResponsePayload = response.Parameters.OriginalResponse;

                if (response.Parameters.Approved)
                {
                    transaction.ConfirmedAt = DateTime.UtcNow;
                    transaction.Payment.ConfirmedAt = transaction.ConfirmedAt;
                    transaction.Payment.TransactionId = response.Parameters.TransactionId;
                    transaction.Payment.Response = transaction.ResponsePayload;
                }
                else
                {
                    transaction.CanceledAt = DateTime.UtcNow;
                }

                await _dbContext.SaveChangesAsync();
                return OperationResult<bool>.Success();
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "PaymentRepository.UpdateTransaction", e);
                return OperationResult<bool>.Failed();
            }
        }
        public async Task<OperationResult<TransactionVM?>> GetTransaction(Guid transactionId)
        {
            try
            {
                //var transaction = await _dbContext
                //    .Transactions
                //    .AsNoTracking()
                //    .SingleOrDefaultAsync(i => i.Id == transactionId);

                var transaction = new Transaction
                {
                    Amount = 100,
                    Id = transactionId,
                    Currency = Abstraction.Enums.Currency.TL,
                    Provider = Abstraction.Enums.ThirdPartyProvider.IsBank,
                    PaymentId = Guid.NewGuid()
                };

                if (transaction == null) return OperationResult<TransactionVM?>.NotFound();

                return OperationResult<TransactionVM?>.Success(transaction.ToVM());
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "PaymentRepository.GetTransaction", e);
                return OperationResult<TransactionVM?>.Failed();
            }
        }
        public async Task<OperationResult<PaymentVM>> RejectPayment(BankCallbackResponse response)
        {
            try
            {
                var payment = await _dbContext
                    .Payments
                    .SingleOrDefaultAsync(i => i.Id == response.PaymentId);
                if (payment == null)

                    payment = new Payment
                    {
                        Amount = response.Parameters.TotalAmount,
                        Currency =Currency.TL,
                        Provider =response.Provider,
                        Response = response.Parameters.OriginalResponse,
                        CreatedAt = DateTime.Now,
                        TransactionId = response.Parameters.TransactionId,
                        Section = PaymentSection.UnKnown,
                        Approver = new User
                        {
                            Name = "Test",
                            Surname = "SurnameTest"
                        }
                    };
                _dbContext.Payments.Add(payment);
                  //  return OperationResult<PaymentVM>.NotFound();
                //if (payment.IsAlreadyLabeled()) return OperationResult<PaymentVM>.Failed();
                await _dbContext.SaveChangesAsync();
                return OperationResult<PaymentVM>.Success(payment.ToVM());
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "PaymentRepository.ApprovePayment", e);
                return OperationResult<PaymentVM>.Failed();
            }
        }
        public async Task<OperationResult<PaymentVM>> ApprovePayment(BankCallbackResponse response)
        {
            try
            {
                var payment = await _dbContext
                    .Payments
                    .SingleOrDefaultAsync(i => i.Id == response.PaymentId);
                if (payment == null) return OperationResult<PaymentVM>.NotFound();

                if (payment.IsAlreadyLabeled()) return OperationResult<PaymentVM>.Failed();

                payment.ConfirmedAt = response.Parameters.CreatedAt;
                payment.TransactionId = response.Parameters.TransactionId;
                payment.Response = response.Parameters.OriginalResponse;

                await _dbContext.SaveChangesAsync();
                return OperationResult<PaymentVM>.Success(payment.ToVM());
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "PaymentRepository.ApprovePayment", e);
                return OperationResult<PaymentVM>.Failed();
            }
        }
    }
}
