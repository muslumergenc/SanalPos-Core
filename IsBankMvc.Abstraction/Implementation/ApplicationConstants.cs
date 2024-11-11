namespace IsBankMvc.Abstraction.Implementation
{
    public record ApplicationConstants
    {
        public const string ApplicationName = "Payment";
        public const string PublicBucket = "Payment-Public";
        public const string ProtectedBucket = "Payment-Protected";
        public const string DefaultUserAgent =
            "Mozilla/5.0 (Windows; U; Windows NT 6.3; Win64; x64; en-US) AppleWebKit/600.5 (KHTML, like Gecko) Chrome/55.0.2671.192 Safari/603";
    }
}
