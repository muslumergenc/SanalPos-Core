using IsBankMvc.Abstraction.Enums;

namespace IsbankCoreMvc.Models
{
    public class UserAuthData
    {
        public Guid ServiceId { get; set; }
        public Guid InstitutionId { get; set; }
        public Guid UserId { get; set; }

        public Guid TokenId { get; set; } = Guid.NewGuid();

        public string Username { get; set; } = string.Empty;
        public string[] Roles { get; set; } = Array.Empty<string>();

        public string Token { get; set; } = string.Empty;

        public UserType UserType { get; set; }
    }
}
