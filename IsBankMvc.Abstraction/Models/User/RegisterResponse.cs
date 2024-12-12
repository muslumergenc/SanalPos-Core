using IsBankMvc.Abstraction.Enums;

namespace IsBankMvc.Abstraction.Models.User
{
    public class RegisterResponse
    {
        public string Token { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public Guid Id { get; set; }
    }
}
