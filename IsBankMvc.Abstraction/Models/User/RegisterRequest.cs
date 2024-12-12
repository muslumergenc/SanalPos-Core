using IsBankMvc.Abstraction.Enums;
namespace IsBankMvc.Abstraction.Models.User
{
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;

        public UserType UserType { get; set; } = UserType.Agent;
    }
}
