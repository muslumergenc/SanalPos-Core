using IsbankCoreMvc.Models;
using IsBankMvc.Abstraction.Models.User;
using System.Security.Claims;

namespace IsbankCoreMvc.Services
{
    public interface ITokenService
    {
        string GenerateToken(UserVM user, DateTime? expiresAt = null);
        ClaimsPrincipal? ExtractClaims(string token);
        UserAuthData ExtractAuthData(Claim[] claims);
    }
}
