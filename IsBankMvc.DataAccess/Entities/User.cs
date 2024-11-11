using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using IsBankMvc.Abstraction.Enums;
using IsBankMvc.Abstraction.Models.User;

namespace IsBankMvc.DataAccess.Entities
{
    [Table("User", Schema = "Account")]
    [Index(nameof(Username), IsUnique = true, Name = "Unique_Username")]
    public class User
    {
        [Key] public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime? BlockedAt { get; set; }
        [Required][MaxLength(128)] public string Username { get; set; } = string.Empty;
        [Required][MaxLength(128)] public string Hash { get; set; } = string.Empty;
        [Required][MaxLength(64)] public string Salt { get; set; } = string.Empty;

        public UserType UserType { get; set; }

        [MaxLength(32)] public string? Name { get; set; }
        [MaxLength(32)] public string? Surname { get; set; }
        [MaxLength(16)] public string? Phone { get; set; }
        [MaxLength(6)] public string? Nationality { get; set; }
        [MaxLength(512)] public string? Address { get; set; }
        [MaxLength(256)] public string? Avatar { get; set; }
        public DateTime? BirthDate { get; set; }
        public int FailedAttempt { get; set; }


        public UserVM ToVM()
        {
            return new UserVM
            {
                Hash = Hash,
                CreatedAt = CreatedAt,
                Username = Username,
                Id = Id,
                BlockedAt = BlockedAt,
                Salt = Salt,
                DeletedAt = DeletedAt,
                FailedAttempt = FailedAttempt,

                Address = Address,
                Avatar = Avatar,
                Phone = Phone,
                Nationality = Nationality,
                Surname = Surname,
                Name = Name,
                BirthDate = BirthDate,
                UserType = UserType
            };
        }

        public static User FromVM(UserVM user)
        {
            return new User
            {
                Hash = user.Hash,
                CreatedAt = user.CreatedAt,
                Username = user.Username,
                Id = user.Id,
                BlockedAt = user.BlockedAt,
                Salt = user.Salt,
                DeletedAt = user.DeletedAt,
                FailedAttempt = user.FailedAttempt,
                Address = user.Address,
                Avatar = user.Avatar,
                Phone = user.Phone,
                Nationality = user.Nationality,
                Surname = user.Surname,
                Name = user.Name,
                BirthDate = user.BirthDate,
                UserType = user.UserType
            };
        }

        public UserMiniVM ToMiniVM()
        {
            return new UserMiniVM
            {
                CreatedAt = CreatedAt,
                Username = Username,
                Id = Id,
                Address = Address,
                Avatar = Avatar,
                Phone = Phone,
                Nationality = Nationality,
                Surname = Surname,
                Name = Name,
                BirthDate = BirthDate,
                UserType = UserType
            };
        }
    }
}
