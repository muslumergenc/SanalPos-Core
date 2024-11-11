namespace IsBankMvc.Provider.IsBank.Fixtures
{
    public static class TemplateForIsBank
    {
        public const string PaymentTemplateV3 = """
                                            <form id="frm-payment-go-to-bank" style="display: none !important" method="post" action="{gateway}">
                                            <input type="hidden" name="clientid" value="{clientId}">
                                            <input type="hidden" name="amount" value="{amount}">
                                            <input type="hidden" name="okurl" value="{okUrl}">
                                            <input type="hidden" name="failUrl" value="{failUrl}">
                                            <input type="hidden" name="TranType" value="{transactionType}">
                                            <input type="hidden" name="Instalment" value="{instalment}" />
                                            <input type="hidden" name="callbackUrl" value="{callbackUrl}">
                                            <input type="hidden" name="currency" value="{currency}">
                                            <input type="hidden" name="rnd" value="{rnd}" />
                                            <input type="hidden" name="storetype" value="{storeType}" />
                                            <input type="hidden" name="lang" value="{lang}" />
                                            <input type="hidden" name="hashAlgorithm" value="{hashAlgorithm}" />
                                            <input type="hidden" name="hash" value="{hash}">
                                            <input type="hidden" name="pan" value="{pan}">
                                            <input type="hidden" name="cv2" value="{cv2}">
                                            <input type="hidden" name="Ecom_Payment_Card_ExpDate_Year" value="{card_exp_year}">
                                            <input type="hidden" name="Ecom_Payment_Card_ExpDate_Month" value="{card_exp_month}">
                                            </form>
                                            <script>document.getElementById('frm-payment-go-to-bank').submit();</script>
                                            """;

        public const string MdStatus3DSecureSignature = "0";
        public const string MdStatusSuccess = "1";
        public const string MdStatusCardNotSuitable = "2";
        public const string MdStatusCardProviderNotSupported = "3";
        public const string MdStatusVerificationAttempt = "4";
        public const string MdStatusCanNotVerify = "5";
        public const string MdStatus3DSecure = "6";
        public const string MdStatusSystemError = "7";
        public const string MdStatusInvalidCardNumber = "8";
        public const string MdStatusMerchantNotRegistered = "9";
    }
}
