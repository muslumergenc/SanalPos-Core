using IsBankMvc.Abstraction.Models.Customer;
using System.ComponentModel.DataAnnotations;

namespace IsBankMvc.DataAccess.Entities
{
    public class Customer
    {
        [Key] public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required][MaxLength(128)] public string Email { get; set; } = string.Empty;
        [MaxLength(16)] public string Phone { get; set; }
        [MaxLength(32)] public string Name { get; set; }
        [MaxLength(32)] public string Surname { get; set; }
        [MaxLength(6)] public string Nationality { get; set; }
        [MaxLength(512)] public string? Address { get; set; }
        public DateTime? BirthDate { get; set; }
        [MaxLength(256)] public string? Avatar { get; set; }

        public virtual ICollection<PaymentCard> PaymentCards { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
        public CustomerVM ToVM()
        {
            return new CustomerVM
            {
                Id = Id,
                Email = Email,
                CreatedAt = CreatedAt,
                Address = Address,
                Avatar = Avatar,
                Phone = Phone,
                Nationality = Nationality,
                Surname = Surname,
                Name = Name,
                BirthDate = BirthDate
            };
        }
    }
}
