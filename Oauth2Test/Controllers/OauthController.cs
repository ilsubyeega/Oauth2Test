using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oauth2Test.Auth;
using Oauth2Test.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Oauth2Test.Controllers
{
	[Route("oauth/")]
	[ApiController]
	public class OauthController : ControllerBase
	{

		[HttpPost("token")]
		[AllowAnonymous]
		public IActionResult GetToken([FromForm] string name, [FromForm] int id)
		{
			// Just testing atm, not going to auth check
			var access = JwtUtil.GenerateToken(id);
			var refresh = JwtUtil.GenerateRefreshToken();
			var oauthuser = new OauthUser
			{
				name = name,
				id = id,
				access_token = access,
				refresh_token = refresh
			};
			Common.ValidatedOauth.Add(oauthuser);
			var result = new OauthResult(oauthuser);
			return Ok(result);
		}
		[HttpPost("token_refresh")]
		[AllowAnonymous]
		public IActionResult RefreshToken([FromForm] string token)
		{
			if (token == null) return Ok("token is null");
			var value = Common.ValidatedOauth.FirstOrDefault(o => o.refresh_token == token);
			if (value != null)
			{
				var access = JwtUtil.GenerateToken((int)value.id);
				var refresh = JwtUtil.GenerateRefreshToken();

				value.access_token = access;
				value.refresh_token = refresh;

				return Ok(new OauthResult(value));
			}
			return Ok("Token not found");
		}

		[HttpGet("test")]
		[AllowAnonymous]
		public IActionResult Test()
		{
			return Ok("Test");
		}

		[HttpGet("test_auth")]
		[Authorize]
		public IActionResult TestAuth()
		{
			return Ok("test");
		}
	}
}
