#region using

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using RaBe.Model;

#endregion

namespace RaBe
{
	internal static class TokenProvider
	{
		internal static string GetToken(Lehrer teacher)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Issuer = "http://localhost:80",
				Audience = "GSO",
				Subject = GetUserClaims(teacher),
				IssuedAt = DateTime.Now,
				Expires = DateTime.Now.AddDays(1),
				Claims = GetUserClaimsAsDictionary(teacher),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Startup.SecretKey),
					SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}

		private static ClaimsIdentity GetUserClaims(Lehrer teacher)
		{
			return new ClaimsIdentity(new List<Claim>
			{
				new Claim("id", teacher.Id.ToString()),
				new Claim("email", teacher.Email),
				new Claim("name", teacher.Name),
				new Claim("random", DateTime.Now.ToString())
			});
		}

		private static Dictionary<string, object> GetUserClaimsAsDictionary(Lehrer teacher)
		{
			return new Dictionary<string, object>
			{
				{"id", teacher.Id.ToString()},
				{"email", teacher.Email},
				{"name", teacher.Name},
				{"random", DateTime.Now.ToString()}
			};
		}
	}
}