using IsBankMvc.Abstraction.Contracts;
using IsBankMvc.Abstraction.Contracts.Payments;
using IsBankMvc.Abstraction.Enums;
using IsBankMvc.Abstraction.Interfaces.Payments;
using IsBankMvc.Abstraction.Models.Payments;
using IsBankMvc.Abstraction.Types;
using IsBankMvc.DataAccess.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Text;
namespace IsBankMvc.Business.Implementation;
public class PaymentService(
    IPaymentRepository paymentRepository,
    ILoggerService loggerService,
    IServiceProvider serviceProvider,
    IJsonService jsonService)
    : IPaymentService
{
    public async Task<OperationResult<PreparePaymentResponse>> PreparePayment(PreparePaymentRequestBase request)
        {
            try
            {
                var paymentOp = await paymentRepository.GetPayment(request.OrderId);
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
                    var transactionOp = await paymentRepository.CreateTransaction(payload, prepareOp.Data!);
                    if (transactionOp.Status != OperationResultStatus.Success)
                        return OperationResult<PreparePaymentResponse>.Failed();
                }
                return prepareOp;
            }
            catch (Exception e)
            {
                await loggerService.Error(e.Message, "PaymentService.PreparePayment", e);
                return OperationResult<PreparePaymentResponse>.Failed();
            }
        }
        public async Task<BankCallbackProcessResponse> BankCallback(BankCallbackProcessRequest process)
        {
            var failedResponse = new BankCallbackProcessResponse();
            try
            {
                var transactionId = process.PaymentId;
                var transactionOp = await paymentRepository.GetTransaction(transactionId);
                if (transactionOp.Status != OperationResultStatus.Success)
                    return failedResponse;
                //var failedUrl = $"{EnvironmentHelper.Get("APP_DOMAIN")}/fail/{transactionOp.Data!.PaymentId}";
                //var successUrl = $"{EnvironmentHelper.Get("APP_DOMAIN")}/success/{transactionOp.Data.PaymentId}";
                var failedUrl = $"https://localhost:7258/is-bank/fail/{transactionOp.Data!.PaymentId}";
                var successUrl = $"https://localhost:7258/is-bank/success/{transactionOp.Data.PaymentId}";
                var provider = GetProvider(process.Provider);
                var op = await provider.BankCallback(process.Parameters);
                process.Success = op.Status == OperationResultStatus.Success && op.Data!.Success;
                op.Data!.PaymentId = transactionOp.Data.PaymentId;
                var updateOp = await paymentRepository.UpdateTransaction(transactionId, op.Data!);
                if (updateOp.Status != OperationResultStatus.Success)
                {
                    var builder = new StringBuilder();
                    builder.Append($"Failed to update transaction response: {transactionId}");
                    builder.Append(jsonService.Serialize(process));
                    await loggerService.Error(builder.ToString(), "PaymentService.BankCallback");
                }
                if (process.Success) await ApprovePayment(op.Data);
                else await RejectPayment(op.Data);
                return new BankCallbackProcessResponse
                {
                    Success = process.Success,
                    Url = process.Success ? successUrl : failedUrl
                };
            }
            catch (Exception e)
            {
                await loggerService.Error(e.Message, "", e);
                return failedResponse;
            }
        }
        public IPaymentProvider GetProvider(ThirdPartyProvider provider)
        {
            //if (provider is ThirdPartyProvider.PayZee)
            //    return _serviceProvider.GetService<IPayZeePaymentProvider>()!;
            //if (provider is ThirdPartyProvider.DenizBank)
            //    return _serviceProvider.GetService<IDenizBankPaymentProvider>()!;
            //if (provider is ThirdPartyProvider.GarantiBank)
            //    return _serviceProvider.GetService<IGarantiBankPaymentProvider>()!;
            if (provider == ThirdPartyProvider.IsBank)
                return serviceProvider.GetService<IIsBankPaymentProvider>()!;
            var instance = serviceProvider.GetService<IIsBankPaymentProvider>()!;
            instance.Provider = provider;
            return instance;
        }

        public async Task ApprovePayment(BankCallbackResponse response)
        {
            await loggerService.Info("ApprovePayment started", "PaymentService.ApprovePayment");
            var op = await paymentRepository.ApprovePayment(response);
            if (op.Status != OperationResultStatus.Success)
                await loggerService.Error(op.Message ?? "ApprovePayment failed", "PaymentService.ApprovePayment");
            await PaymentPostOperations(true, response.PaymentId, response.Parameters);
        }

        private async Task PaymentPostOperations(bool success, Guid paymentId, BankCallbackParameters? parameters)
        {
            try
            {
                await loggerService.Log($"Started for payment {paymentId}", "PaymentService.PaymentPostOperations");
                var op = await paymentRepository.GetPayment(paymentId);
                if (op.Status != OperationResultStatus.Success)
                {
                    await loggerService.Log("Payment not found", "PaymentService.PaymentPostOperations");
                    return;
                }
                //var customerOp = await _paymentRepository.GetCustomer(op.Data!.CustomerId);
                //if (customerOp.Status != OperationResultStatus.Success)
                //{
                //    await _loggerService.Log("Customer not found", "PaymentService.PaymentPostOperations");
                //    return;
                //}
                //var url = GetLogo(op.Data!.Section);
                //    var replaceValues = new Dictionary<string, string>
                //{
                //    { "[CUSTOMER_NAME]", customerOp.Data.Name + " " + customerOp.Data.Surname },
                //    { "[CUSTOMER_EMAIL]", customerOp.Data.Email },
                //    { "[CUSTOMER_ADDRESS]", customerOp.Data.Address },
                //    { "[TOTAL_AMOUNT]", op.Data.Amount.ToString(_culture) },
                //    { "[CURRENCY]", op.Data.Currency.ToString() },
                //    { "[ORDER_ID]", op.Data.Id.ToString() },
                //    { "[BANK_TRANSACTION]", op.Data.TransactionId ?? "---------------" },
                //    { "[DATE]", op.Data?.CreatedAt.Value.ToString() },
                //    { "[BANK_MESSAGE]", parameters.ResponseMessage },
                //    { "[CONFIRMATION_NUMBER]", op.Data.Id.ToString() },
                //    { "[LOGO]", url }
                //};

                if (success)
                {
                    await loggerService.Log("Confirming Payment", "PaymentService.PaymentPostOperations");
                    //await _messagingService.OnPaymentConfirm(new OnPaymentConfirmedVM
                    //{
                    //    Payment = op.Data!,
                    //    Parameters = replaceValues,
                    //    Email = customerOp.Data!.Email
                    //});
                    return;
                }

                await loggerService.Log("Rejecting Payment", "PaymentService.PaymentPostOperations");
                //await _messagingService.OnPaymentReject(new OnPaymentRejectVM
                //{
                //    Payment = op.Data!,
                //    Parameters = replaceValues,
                //    Email = customerOp.Data!.Email
                //});
            }
            catch (Exception e)
            {
                await loggerService.Error(e.Message, "PaymentService.PaymentPostOperations", e);
            }
        }

        public async Task<OperationResult<PaymentVM>> RejectPayment(BankCallbackResponse response)
        {
            await loggerService.Info("RejectPayment started", "PaymentService.RejectPayment");
            var op = await paymentRepository.RejectPayment(response);
            if (op.Status != OperationResultStatus.Success)
                await loggerService.Error(op.Message ?? "RejectPayment failed", "PaymentService.RejectPayment");
            await PaymentPostOperations(false, response.PaymentId, response.Parameters);
            return op;
        }

    }
