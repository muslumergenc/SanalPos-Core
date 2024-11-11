namespace IsBankMvc.Abstraction.Models.Payments
{
    public class BankCallbackParameters
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public string OriginalResponse { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string ReferenceId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Approved { get; set; }
        public bool TxnResult { get; set; }
        public string AuthCode { get; set; } = string.Empty;
    }
}