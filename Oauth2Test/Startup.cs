using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Oauth2Test
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			

			services.AddControllers();
			services.AddLogging(l =>
			{
				l.ClearProviders();
				// Debug
				l.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
				l.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
				l.AddConsole();
			});


			services.AddAuthentication(cfg =>
			{
				cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(opt =>
			{
				opt.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateAudience = false,
					ValidateIssuer = false,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Common.GetKey()),
					ValidateLifetime = true,
					ClockSkew = TimeSpan.FromMinutes(0.5)
				};

				opt.Events = new JwtBearerEvents
				{
					OnTokenValidated = async context =>
					{
						Console.WriteLine("Log");
						var token = (JwtSecurityToken)context.SecurityToken;
						var obj = Common.ValidatedOauth.FirstOrDefault(o => o.access_token == token.RawData);
						if (obj == null)
						{
							context.Fail("Unauthorized");
							
						} else
						{
							Console.WriteLine(token.RawData);
							Console.WriteLine(obj.id);

							context.Success();
						}
					},
					OnAuthenticationFailed = context =>
					{
						if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
						{
							context.Response.Headers.Add("Token-Expired", "true");
						}
						
						return Task.CompletedTask;
					}
				};

			});


			services.AddAuthorization();

			services.Configure<FormOptions>(
				x =>
				{
					x.ValueLengthLimit = int.MaxValue;
					x.MultipartBodyLengthLimit = int.MaxValue;
					x.MemoryBufferThreshold = int.MaxValue;
					x.BufferBodyLengthLimit = int.MaxValue;
					x.MultipartBoundaryLengthLimit = int.MaxValue;
					x.MultipartHeadersLengthLimit = int.MaxValue;
				}
			);

			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy",
					builder => builder
						.AllowAnyMethod()
						.AllowCredentials()
						.SetIsOriginAllowed((host) => true)
						.AllowAnyHeader());
			});

		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseWebSockets();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
