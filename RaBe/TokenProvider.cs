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
	public static class TokenProvider
	{
		public static JwtSecurityToken GetToken(Lehrer teacher)
		{
			return new JwtSecurityToken(
				"http://localhost:80",
				"GSO",
				GetUserClaims(teacher),
				new DateTime(),
				new DateTime().AddDays(1),
				new SigningCredentials(new SymmetricSecurityKey(Startup.SecretKey),
					SecurityAlgorithms.HmacSha256Signature)
			);
		}

		private static IEnumerable<Claim> GetUserClaims(Lehrer teacher)
		{
			return new List<Claim>
			{
				new Claim("id", teacher.Id.ToString()),
				new Claim("email", teacher.Email),
				new Claim("name", teacher.Name)
			};
		}
	}
}