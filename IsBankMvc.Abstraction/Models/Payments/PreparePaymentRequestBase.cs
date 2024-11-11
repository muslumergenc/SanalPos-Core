using System.ComponentModel.DataAnnotations;

namespace IsBankMvc.Abstraction.Models.Payments
{
    public class PreparePaymentRequestBase
    {
        public string RequestIp { get; set; } = string.Empty;
        [Required] public Guid OrderId { get; set; }
        public string CardAlias { get; set; } = string.Empty;
        [Required] public string CardNumber { get; set; } = string.Empty;
        [Required] public string ExpiryDateMonth { get; set; } = string.Empty;
        [Required] public string ExpiryDateYear { get; set; } = string.Empty;
        [Required] public string CardHolderName { get; set; } = string.Empty;
        [Required] public string Cvv { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Language { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? District { get; set; } = string.Empty;
        public string? CompanyName { get; set; } = string.Empty;
    }
}
