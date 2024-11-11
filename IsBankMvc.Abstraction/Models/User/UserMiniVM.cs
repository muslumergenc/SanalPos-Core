using IsBankMvc.Abstraction.Enums;

namespace IsBankMvc.Abstraction.Models.User
{
    public class UserMiniVM
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Phone { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
