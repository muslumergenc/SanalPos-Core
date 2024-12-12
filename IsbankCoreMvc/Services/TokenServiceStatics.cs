using IsBankMvc.Abstraction.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IsbankCoreMvc.Services
{
    public class TokenServiceStatics
    {
        public const string TokenSchema = "Bearer ";
        public const string InstitutionId = "InstitutionId";
        public const string ServiceId = "ServiceId";
        public const string Roles = "Roles";
        public static readonly string _issuer;
        public static readonly string _secret;
        public static readonly byte[] _key;
        public static readonly IJsonService _jsonService;
        public static readonly SymmetricSecurityKey _symetrics;


        static TokenServiceStatics()
        {
            _issuer = "generate-possystem";
            _secret = "bl7fqy1eMHiPNjNc7ARKxEv7C9nNUXE7gpGcltekc9rVgvkN0U";
            _key = Encoding.UTF8.GetBytes(_secret);
            _symetrics = new SymmetricSecurityKey(_key);
        }

        public static TokenValidationParameters GetParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _issuer,
                IssuerSigningKey = _symetrics
            };
        }
    }
}
