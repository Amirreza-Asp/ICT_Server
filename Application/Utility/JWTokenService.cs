using Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Utility
{
    public static class JWTokenService
    {
        public static SymmetricSecurityKey Key =>
           new SymmetricSecurityKey(Encoding.UTF8.GetBytes("dklfnasdklsdjlfsdjlfnlfnsdkl564564fnklefnsdklfnasdklfnsdjlfsds455wofwifewfoiwf"));

        public static string GenerateToken(String nationalId, String roleId, String ipAddress, Guid? companyId)
        {
            var claims = new List<Claim>
            {
                new Claim(AppClaims.NationalId ,nationalId),
                new Claim(AppClaims.Role , roleId),
                new Claim(AppClaims.IpAddress , ipAddress),
                new Claim(AppClaims.CompanyId , companyId.HasValue ? companyId.Value.ToString() : Guid.Empty.ToString())
            };

            var cred = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = cred
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static String GetUserNationalId(this ClaimsIdentity claims)
        {
            return claims.FindFirst(AppClaims.NationalId).Value;
        }

        public static String GetRole(this ClaimsIdentity claims)
        {
            return claims.FindFirst(AppClaims.Role).Value;
        }

        public static String GetFullName(this ClaimsIdentity claims)
        {
            return claims.FindFirst(AppClaims.FullName).Value;
        }

        public static DateTime GetTokenExpirationTime(this ClaimsIdentity claims)
        {
            var tokenExp = claims.FindFirst(claim => claim.Type.Equals("exp")).Value;
            var ticks = long.Parse(tokenExp);
            return ConvertFromUnixTimestamp(ticks);
        }

        private static DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }
}
