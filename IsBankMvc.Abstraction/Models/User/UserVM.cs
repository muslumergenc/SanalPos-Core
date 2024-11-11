namespace IsBankMvc.Abstraction.Models.User
{
    public class UserVM: UserMiniVM
    {
        public string Hash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public int FailedAttempt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime? BlockedAt { get; set; }
    }
}
