using IsBankMvc.Abstraction.Enums;

namespace IsBankMvc.Abstraction.Models.User
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public Guid Id { get; set; }
    }
}
