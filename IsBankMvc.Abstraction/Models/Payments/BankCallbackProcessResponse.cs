namespace IsBankMvc.Abstraction.Models.Payments
{
    public class BankCallbackProcessResponse
    {
        public bool Success { get; set; }
        public string Url { get; set; } = string.Empty;

        public string Message { get; set; }
    }
}
