using IsBankMvc.Abstraction.Contracts;
using IsBankMvc.Abstraction.Contracts.Payments;
using IsBankMvc.Abstraction.Enums;
using IsBankMvc.Abstraction.Interfaces.Payments;
using IsBankMvc.Abstraction.Models.Payments;
using IsBankMvc.Abstraction.Types;
using IsBankMvc.DataAccess.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace IsBankMvc.Business.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILoggerService _loggerService;
        private readonly IServiceProvider _serviceProvider;
        public PaymentService(IPaymentRepository paymentRepository, ILoggerService loggerService, IServiceProvider serviceProvider)
        {
            _paymentRepository = paymentRepository;
            _loggerService = loggerService;
            _serviceProvider = serviceProvider;
        }

        public async Task<OperationResult<PreparePaymentResponse>> PreparePayment(PreparePaymentRequestBase request)
        {
            try
            {
                var paymentOp = await _paymentRepository.GetPayment(request.OrderId);

                if (paymentOp.Status != OperationResultStatus.Success)
                    return OperationResult<PreparePaymentResponse>.NotFound();

                if (paymentOp.Data!.IsAlreadyLabeled()) return OperationResult<PreparePaymentResponse>.Failed();

                //var customer = await _paymentRepository.GetCustomer(paymentOp.Data.CustomerId);

                //if (customer.Status != OperationResultStatus.Success)
                //    return OperationResult<PreparePaymentResponse>.NotFound();

                var culture = new CultureInfo(request.Language ?? "tr-TR").TwoLetterISOLanguageName;
                var provider = GetProvider(paymentOp.Data!.Provider);
                var payload = new PreparePaymentRequest
                {
                    OrderId = request.OrderId,
                    TransactionId = IncrementalGuid.NewId(),
                    Provider = paymentOp.Data.Provider,

                    Currency = paymentOp.Data.Currency,
                    CustomerId = paymentOp.Data.CustomerId,
                    TotalAmount = paymentOp.Data.Amount,
                    RequestIp = request.RequestIp,
                    Cvv = request.Cvv,
                    Description = request.Description,
                    CardAlias = request.CardAlias,
                    CardNumber = request.CardNumber,
                    CardHolderName = request.CardHolderName,
                    ExpiryDateMonth = request.ExpiryDateMonth,
                    ExpiryDateYear = request.ExpiryDateYear,
                    Language = culture.ToLower(),
                    Address = request.Address,
                    City = request.City,
                    District = request.District,
                    CompanyName = request.CompanyName,

                    // payload posted to payment provider
                    ExtraData = null,

                    // stored customer information
                    //Customer = customer.Data!,

                    // provided customer information
                    // TODO: also use this email if it is not the same as customer email
                    Email = request.Email,
                    Name = request.Name,
                    Surname = request.Surname,
                    Phone = request.Phone
                };
                var prepareOp = await provider.PreparePayment(payload);

                if (prepareOp.Status == OperationResultStatus.Success)
                {
                    var transactionOp = await _paymentRepository.CreateTransaction(payload, prepareOp.Data!);
                    if (transactionOp.Status != OperationResultStatus.Success)
                        return OperationResult<PreparePaymentResponse>.Failed();
                }

                return prepareOp;
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "PaymentService.PreparePayment", e);
                return OperationResult<PreparePaymentResponse>.Failed();
            }
        }
        public IPaymentProvider GetProvider(ThirdPartyProvider provider)
        {
            if (provider == ThirdPartyProvider.IsBank)
            {
                var Provider = _serviceProvider.GetService<IIsBankPaymentProvider>();
                if (Provider == null)
                {
                    throw new InvalidOperationException("IIsBankPaymentProvider servisi servis sağlayıcıda kayıtlı değil.");
                }
                return Provider;
            }

            throw new ArgumentException("Desteklenmeyen sağlayıcı türü", nameof(provider));
        }

       // public IPaymentProvider GetProvider(ThirdPartyProvider provider)
       // {
            //if (provider==ThirdPartyProvider.IsBank)
            //    return _serviceProvider.GetService<IIsBankPaymentProvider>()!;
            //if (provider is ThirdPartyProvider.PayZee)
            //    return _serviceProvider.GetService<IPayZeePaymentProvider>()!;
            //if (provider is ThirdPartyProvider.DenizBank)
            //    return _serviceProvider.GetService<IDenizBankPaymentProvider>()!;
            //if (provider is ThirdPartyProvider.GarantiBank)
            //    return _serviceProvider.GetService<IGarantiBankPaymentProvider>()!;
            //var instance = _serviceProvider.GetService<IIsBankPaymentProvider>()!;
           // instance.Provider = provider;
           // return instance;
       // }
    }
}
