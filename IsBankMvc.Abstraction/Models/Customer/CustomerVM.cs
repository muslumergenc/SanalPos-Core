namespace IsBankMvc.Abstraction.Models.Customer
{
    public class CustomerVM
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Phone { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
