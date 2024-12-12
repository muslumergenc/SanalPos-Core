using IsBankMvc.Abstraction.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsBankMvc.Abstraction.Models.User
{
    public class UserUpdateRequest:ProfileUpdateRequest
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public UserType UserType { get; set; }
    }
    public class ProfileUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
    }
    public record ResetPasswordRequest
    {
        public string Password { get; set; }
        public Guid UserId { get; set; }
    }
}
