namespace IsBankMvc.Provider.IsBank.Models
{
    public class IsBankPreparePaymentRequestV3
    {
        public string? Lang { get; set; }
        public string? ClientId { get; set; }
        public string? Amount { get; set; }
        public string? OkUrl { get; set; }
        public string? FailUrl { get; set; }
        public string? TransactionType { get; set; }
        public string? Installment { get; set; }
        public string? Rnd { get; set; }
        public string? CallbackUrl { get; set; }
        public string? Currency { get; set; }
        public string? Storekey { get; set; }
        public string? StoreType { get; set; }
        public string? Pan { get; set; }
        public string? PanExpireMonth { get; set; }
        public string? PanExpireYear { get; set; }
        public string? Cvv { get; set; }
        public string? Hash { get; set; }
        public string HashAlgorithm { get; set; } = "ver3";
    }
}
