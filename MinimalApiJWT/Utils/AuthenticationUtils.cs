using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace MinimalApiJWT.Utils
{
    public class AuthenticationUtils
    {
        public static string GenerateToken(IConfiguration configuration)
        {
            var encondedKey = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
            var securityKey = new SymmetricSecurityKey(encondedKey);

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];

            var token = new JwtSecurityToken(issuer: issuer,
                                             audience: audience,
                                             signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }
    }
}
