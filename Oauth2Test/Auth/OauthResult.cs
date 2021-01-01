using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oauth2Test.Auth
{
	public class OauthResult
	{
		public string? token_type { get; set; }
		public int? expires_in { get; set; }
		public string? access_token { get; set; }
		public string? refresh_token { get; set; }

		public OauthResult() { }

		public OauthResult(OauthUser user) 
		{
			token_type = "Bearer";
			expires_in = 86400;
			access_token = user.access_token;
			refresh_token = user.refresh_token;
		}
	}

}
