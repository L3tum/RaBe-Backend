using Microsoft.IdentityModel.Tokens;
using RaBe.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RaBe
{
    public static class TokenProvider
    {
        public static JwtSecurityToken GetToken(Lehrer teacher)
        {
            return new JwtSecurityToken(
                issuer: "http://localhost:80",
                audience: "GSO",
                claims: GetUserClaims(teacher),
                notBefore: new DateTime(),
                expires: new DateTime().AddDays(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Startup.SecretKey),
                                SecurityAlgorithms.HmacSha256Signature)
                );
        }

        private static IEnumerable<Claim> GetUserClaims(Lehrer teacher)
        {
            return new List<Claim>()
            {
                new Claim("id", teacher.Id.ToString()),
                new Claim("email", teacher.Email),
                new Claim("name", teacher.Name)
            };
        }
    }
}
