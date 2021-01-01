using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Oauth2Test.Utils
{
	public static class JwtUtil
	{
		public static string GenerateToken(int id, Int32 expiresec = 86400)
		{
			var key = new SymmetricSecurityKey(Common.GetKey());
			var jwt = new JwtSecurityToken(issuer: "Sakamoto",
				audience: "Everyone",
				claims: new Claim[] { 
					new Claim(ClaimTypes.Name, id.ToString())
				},
				notBefore: DateTime.UtcNow,
				expires: DateTime.UtcNow.AddSeconds(expiresec),
				signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
			);
			return new JwtSecurityTokenHandler().WriteToken(jwt);
		}
		public static string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				return Convert.ToBase64String(randomNumber);
			}
		}
	}
}
