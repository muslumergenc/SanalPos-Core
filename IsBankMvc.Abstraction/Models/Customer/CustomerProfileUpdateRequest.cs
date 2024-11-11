namespace IsBankMvc.Abstraction.Models.Customer
{
    public class CustomerProfileUpdateRequest
    {
        public DateTime? BirthDate { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string? Address { get; set; }
        public string Nationality { get; set; }
        public string Name { get; set; }
    }
}
