using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Oauth2Test
{
	public static class Common
	{
		private static byte[] _key = null;
		public static byte[] GetKey()
		{
			if (_key == null)
			{
				var b64 = File.ReadAllText("public.key");
				_key = Convert.FromBase64String(b64);
			}
			return _key;
		}
		public static List<OauthUser> ValidatedOauth = new List<OauthUser>();
	}
	public class OauthUser
	{
		public string? name;
		public int? id;
		public string? access_token;
		public string? refresh_token;
	}
}
