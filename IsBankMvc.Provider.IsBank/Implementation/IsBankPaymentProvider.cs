using IsBankMvc.Abstraction.Contracts;
using IsBankMvc.Abstraction.Contracts.Payments;
using IsBankMvc.Abstraction.Enums;
using IsBankMvc.Abstraction.Models.Payments;
using IsBankMvc.Abstraction.Types;
using IsBankMvc.Provider.IsBank.Fixtures;
using IsBankMvc.Provider.IsBank.Helpers;
using IsBankMvc.Provider.IsBank.Models;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
namespace IsBankMvc.Provider.IsBank.Implementation;
public class IsBankPaymentProvider : IIsBankPaymentProvider
    {
        private readonly string _chargeType;
        private readonly string _clientId;
        private readonly string _failUrl;
        private readonly string _payUrl;
        private readonly string _storeKey;
        private readonly string _storeType;
        private readonly string _successUrl;
        private readonly IJsonService _jsonService;
        private readonly ILoggerService _loggerService;
        private readonly Dictionary<string, string> _currencyMap = [];
        private readonly CultureInfo _culture = new("tr-TR", false);
        public IsBankPaymentProvider(ILoggerService loggerService, IJsonService jsonService)
        {
            _loggerService = loggerService;
            _jsonService = jsonService;
            _payUrl = "https://istest.asseco-see.com.tr/fim/est3Dgate";
            _successUrl = "https://localhost:7258/is-bank/success/:paymentId";
            _failUrl = "https://localhost:7258/is-bank/fail/:paymentId";
            _storeKey = "TEST3232";
            _storeType = "3D_PAY";
            _clientId = "*************";
            _chargeType = "Auth";
            _currencyMap.Add("EUR", "978");
            _currencyMap.Add("GBP", "826");
            _currencyMap.Add("JPY", "392");
            _currencyMap.Add("RUB", "643");
            _currencyMap.Add("USD", "840");
            _currencyMap.Add("TRY", "949");
            _currencyMap.Add("TL", "949");
            _currencyMap.Add("CAD", "124");
        }

        public ThirdPartyProvider Provider
        {
            set
            {
                /* ignore set! */
            }
            get => ThirdPartyProvider.IsBank;
        }
        public async Task<OperationResult<BankCallbackResponse>> BankCallback(Dictionary<string, string> parameters)
        {
            try
            {
                await _loggerService.Info("BankCallback started", "IsBankPaymentProvider.BankCallback");
                var response = new BankCallbackResponse{
                    Success = false,
                    Provider = ThirdPartyProvider.IsBank,
                    Parameters = ParseParameters(parameters)
                };
                var verifiedOp = await VerifyPayment(parameters);
                if (verifiedOp.Status != OperationResultStatus.Success){
                    await _loggerService.Info($"BankCallback rejected: {verifiedOp.Message}","IsBankPaymentProvider.BankCallback");
                    return OperationResult<BankCallbackResponse>.Rejected(data: response, message: verifiedOp.Message);
                }
                response.Parameters = ParseParameters(parameters);
                response.Success = response.Parameters.Approved;
                var message = $"BankCallback payment approved: {response.Success}";
                await _loggerService.Info(message, "IsBankPaymentProvider.BankCallback");
                return OperationResult<BankCallbackResponse>.Success(response);
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "IsBankPaymentProvider.BankCallback", e);
                return OperationResult<BankCallbackResponse>.Failed();
            }
        }
        public async Task<OperationResult<PreparePaymentResponse>> PreparePayment(PreparePaymentRequest request)
        {
            try
            {
                var req = new IsBankPreparePaymentRequestV3
                {
                    ClientId = _clientId,
                    Amount = NumberHelpers.ToString(request.TotalAmount),
                    FailUrl = _failUrl.Replace(":paymentId", request.TransactionId.ToString()),
                    OkUrl = _successUrl.Replace(":paymentId", request.TransactionId.ToString()),
                    TransactionType = _chargeType,
                    Installment = string.Empty,
                    CallbackUrl = string.Empty,
                    Currency = GetCurrencyCode(request.Currency.ToString()),
                    Rnd = GenerateRandom(),
                    StoreType = _storeType,
                    Lang = request.Language,
                    HashAlgorithm = "ver3",
                    Hash = "",
                    Pan = request.CardNumber,
                    Cvv = request.Cvv,
                    PanExpireYear = request.ExpiryDateYear,
                    PanExpireMonth = request.ExpiryDateMonth,
                    Storekey = _storeKey
                };
                GenerateV3Hash(req);
                var markup = new StringBuilder(TemplateForIsBank.PaymentTemplateV3)
                    .Replace("{gateway}", _payUrl)
                    .Replace("{clientId}", req.ClientId)
                    .Replace("{amount}", req.Amount)
                    .Replace("{okUrl}", req.OkUrl)
                    .Replace("{failUrl}", req.FailUrl)
                    .Replace("{transactionType}", req.TransactionType)
                    .Replace("{instalment}", req.Installment)
                    .Replace("{callbackUrl}", req.CallbackUrl)
                    .Replace("{currency}", req.Currency)
                    .Replace("{rnd}", req.Rnd)
                    .Replace("{storeType}", req.StoreType)
                    .Replace("{lang}", req.Lang)
                    .Replace("{hashAlgorithm}", req.HashAlgorithm)
                    .Replace("{hash}", req.Hash)
                    .Replace("{pan}", req.Pan)
                    .Replace("{cv2}", req.Cvv)
                    .Replace("{card_exp_year}", req.PanExpireYear)
                    .Replace("{card_exp_month}", req.PanExpireMonth);
                return OperationResult<PreparePaymentResponse>.Success(new PreparePaymentResponse{
                    Markup = markup.ToString(),
                    Mode = PaymentGatewayMode.Form,
                    Provider = ThirdPartyProvider.IsBank
                });
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "IsBankPaymentProvider.PreparePayment", e);
                return OperationResult<PreparePaymentResponse>.Failed();
            }
        }
        public BankCallbackParameters ParseParameters(Dictionary<string, string> parameters)
        {
            try
            {
                return new BankCallbackParameters
                {
                    OriginalResponse = _jsonService.Serialize(parameters),
                    AuthCode = TryGetValue(parameters, "AuthCode"),
                    Approved = TryGetValue(parameters, "ProcReturnCode") == "00",
                    ResponseCode = TryGetValue(parameters, "Response"),
                    ResponseMessage = TryGetValue(parameters, "ErrMsg"),
                    TransactionId = TryGetValue(parameters, "TransId"),
                    ReferenceId = TryGetValue(parameters, "HostRefNum"),
                    TotalAmount = NumberHelpers.ParseMoney(TryGetValue(parameters, "amount", "0"))
                };
            }
            catch (Exception e)
            {
                _loggerService.Error(e.Message, "IsBankPaymentProvider.ParseParameters", e)
                    .GetAwaiter().GetResult();
                return new BankCallbackParameters();
            }
        }
        public async Task<OperationResult<bool>> VerifyPayment(Dictionary<string, string> parameters)
        {
            var json = _jsonService.Serialize(parameters);
            await _loggerService.Info(json, "IsBankPaymentProvider.VerifyPayment");

            try
            {
                var errorKey = "ErrMsg";
                var mdStatus = TryGetValue(parameters, "mdStatus");
                string errorMessage;
                switch (mdStatus)
                {
                    case TemplateForIsBank.MdStatusSuccess:
                        return OperationResult<bool>.Success();
                    case TemplateForIsBank.MdStatus3DSecureSignature:
                        errorMessage = TryGetValue(parameters, errorKey, "MdStatus3DSecureSignature");
                        return OperationResult<bool>.Failed(errorMessage);
                    case TemplateForIsBank.MdStatusCardNotSuitable:
                        errorMessage = TryGetValue(parameters, errorKey, "MdStatusCardNotSuitable");
                        return OperationResult<bool>.Failed(errorMessage);
                    case TemplateForIsBank.MdStatusCardProviderNotSupported:
                        errorMessage = TryGetValue(parameters, errorKey, "MdStatusCardProviderNotSupported");
                        return OperationResult<bool>.Failed(errorMessage);
                    case TemplateForIsBank.MdStatusVerificationAttempt:
                        errorMessage = TryGetValue(parameters, errorKey, "MdStatusVerificationAttempt");
                        return OperationResult<bool>.Failed(errorMessage);
                    case TemplateForIsBank.MdStatusCanNotVerify:
                        errorMessage = TryGetValue(parameters, errorKey, "MdStatusCanNotVerify");
                        return OperationResult<bool>.Failed(errorMessage);
                    case TemplateForIsBank.MdStatus3DSecure:
                        errorMessage = TryGetValue(parameters, errorKey, "MdStatus3DSecure");
                        return OperationResult<bool>.Failed(errorMessage);
                    case TemplateForIsBank.MdStatusSystemError:
                        errorMessage = TryGetValue(parameters, errorKey, "MdStatusSystemError");
                        return OperationResult<bool>.Failed(errorMessage);
                    case TemplateForIsBank.MdStatusInvalidCardNumber:
                        errorMessage = TryGetValue(parameters, errorKey, "MdStatusInvalidCardNumber");
                        return OperationResult<bool>.Failed(errorMessage);
                    case TemplateForIsBank.MdStatusMerchantNotRegistered:
                        errorMessage = TryGetValue(parameters, errorKey, "MdStatusMerchantNotRegistered");
                        return OperationResult<bool>.Failed(errorMessage);

                }

                var retrievedHash = TryGetValue(parameters, "HASH");
                var builder = new StringBuilder();
                var storeKey = FixText(_storeKey);

                var sortedKeys = parameters.Keys.Order().ToArray();
                foreach (var key in sortedKeys)
                {
                    var escapedValue = FixText(parameters[key]);
                    var lowerValue = key.ToLower(_culture);

                    if (!lowerValue.Equals("encoding") && !lowerValue.Equals("hash"))
                    {
                        builder.Append(escapedValue);
                        builder.Append("|");
                    }
                }

                builder.Append(storeKey);

                var hashVal = builder.ToString();
                var actualHash = GenerateHash(hashVal);

                if (actualHash.Equals(retrievedHash))
                {
                    await _loggerService.Info("Hash verified successfully", "IsBankPaymentProvider.VerifyPayment");
                    return OperationResult<bool>.Success();
                }

                await _loggerService.Info("Hash failed!", "IsBankPaymentProvider.VerifyPayment");
                return OperationResult<bool>.Failed("Hash mis-matched!");
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "IsBankPaymentProvider.VerifyPayment", e);
                return OperationResult<bool>.Failed();
            }
        }
        #region Private Methods
        private string FixText(string input)
        {
            return input
                .Replace("\\", "\\\\")
                .Replace("|", "\\|");
        }

        private string TryGetValue(Dictionary<string, string> dic, string key, string def = "")
        {
            return dic.ContainsKey(key) ? dic[key] : def;
        }

        private string GenerateRandom()
        {
            return new StringBuilder(Convert.ToBase64String(Guid.NewGuid().ToByteArray()))
                .Replace("=", "")
                .Replace("+", "")
                .ToString();
        }

        private string GetCurrencyCode(string currency)
        {
            currency = currency.Trim().ToUpper();
            if (_currencyMap.TryGetValue(currency, out var code)) return code;

            throw new NotImplementedException();
        }

        private void GenerateV3Hash(IsBankPreparePaymentRequestV3 req)
        {
            var parts = new[]
            {
            req.Amount,
            req.CallbackUrl,
            req.ClientId,
            req.Currency,
            req.Cvv,
            req.PanExpireMonth,
            req.PanExpireYear,
            req.FailUrl,
            req.HashAlgorithm,
            req.Installment,
            req.Lang,
            req.OkUrl,
            req.Pan,
            req.Rnd,
            req.StoreType,
            req.TransactionType,
            req.Storekey
        };
            var hashstr = string.Join('|', parts);
            req.Hash = GenerateHash(hashstr);
        }
        private string GenerateHash(string pattern)
        {
            byte[] inputbytes;
            using (var sha = SHA512.Create())
            {
                var hashbytes = Encoding.UTF8.GetBytes(pattern);
                inputbytes = sha.ComputeHash(hashbytes);
            }
            return Convert.ToBase64String(inputbytes);
        }
        #endregion
    }
}