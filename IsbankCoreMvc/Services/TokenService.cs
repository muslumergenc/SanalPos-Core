using IsbankCoreMvc.Models;
using IsBankMvc.Abstraction.Contracts;
using IsBankMvc.Abstraction.Enums;
using IsBankMvc.Abstraction.Models.User;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IsbankCoreMvc.Services
{
    public class TokenService : ITokenService
    {
        private readonly IJsonService _jsonService;

        public TokenService(IJsonService jsonService)
        {
            _jsonService = jsonService;
        }
        public string GenerateToken(UserVM user, DateTime? expiresAt = null)
        {
            var credentials = new SigningCredentials(TokenServiceStatics._symetrics, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Typ, user.UserType.ToString()),
        };
            var token = new JwtSecurityToken(TokenServiceStatics._issuer, TokenServiceStatics._issuer, claims,
                expires: expiresAt ?? DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials);
            return $"{TokenServiceStatics.TokenSchema}{new JwtSecurityTokenHandler().WriteToken(token)}";
        }
        public ClaimsPrincipal? ExtractClaims(string token)
        {
            try
            {
                token = token.Replace(TokenServiceStatics.TokenSchema, string.Empty);
                var validator = new JwtSecurityTokenHandler();
                if (!validator.CanReadToken(token)) return null;
                IdentityModelEventSource.ShowPII = true;
                return validator.ValidateToken(token, TokenServiceStatics.GetParameters(), out _);
            }
            catch
            {
                return null;
            }
        }
        public UserAuthData ExtractAuthData(Claim[] claims)
        {
            var userAuthData = new UserAuthData();
            var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userId == null) throw new UnauthorizedAccessException();
            var username = claims.First(c => c.Type == JwtRegisteredClaimNames.Sub);
            var tokenId = claims.First(c => c.Type == JwtRegisteredClaimNames.Jti);
            var userType = claims.First(c => c.Type == JwtRegisteredClaimNames.Typ);
            userAuthData.UserId = Guid.Parse(userId.Value);
            userAuthData.TokenId = Guid.Parse(tokenId.Value);
            userAuthData.Username = username.Value;
            userAuthData.UserType = Enum.Parse<UserType>(userType.Value);
            return userAuthData;
        }
    }
}
