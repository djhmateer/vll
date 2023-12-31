﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace VLL.Web
{
	//public class Helper
	//{
	//    // https://stackoverflow.com/a/24182340/26086
	//    public class AuthorizeRolesAttribute : AuthorizeAttribute
	//    {
	//        public AuthorizeRolesAttribute(params string[] roles) => Roles = string.Join(",", roles);
	//    }

	//}
	public static class StringExtensions
	{
		public static string Ellipsis(this string s, int charsToDisplay)
		{
			if (!string.IsNullOrWhiteSpace(s))
				return s.Length <= charsToDisplay ? s : new string(s.Take(charsToDisplay).ToArray()) + "...";
			return String.Empty;
		}
	}

	public static class DateHelper
	{
		public static DateTime UtcToGmt(this DateTime dateTime)
		{
			// GMT Standard Time should correct for daylight savings
			// http://www.cryer.co.uk/brian/csharp/localisation_in_a_nutshell.htm#daylight_saving
			return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"));
		}
	}


	public static class LogHelper
	{
		public static async Task LogToDbAndLog(string message, int jobId)
		{
			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

			Serilog.Log.Information(message);

			await Db.InsertLog(connectionString, jobId, message);
		}
	}

	// https://stackoverflow.com/a/23182807/26086
	public static class FileHelper
	{
		public static string ReplaceInvalidChars(string filename)
		{
			return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
		}
	}

	// Tus
	// /files/1221213123
	// can't see this being hit in normal use.
	// maybe for non evergreen browsers
	//public static class DownloadFileEndpoint
	//{
	//    public static async Task HandleRoute(HttpContext context)
	//    {
	//        await Task.Delay(100);
	//        Log.Warning("******************** Inside HandleRoute - trying to figure out where this is called");


	//        //var config = context.RequestServices.GetRequiredService<DefaultTusConfiguration>();

	//        //if (!(config.Store is ITusReadableStore store))
	//        //{
	//        //    return;
	//        //}

	//        //Log.Information("Tusstore okay");
	//        //var fileId = (string)context.Request.RouteValues["fileId"];
	//        //Log.Information($"fileId is {fileId}");

	//        //var file = await store.GetFileAsync(fileId, context.RequestAborted);
	//        //Log.Information($"fileid is {file?.Id}");

	//        //if (file == null)
	//        //{
	//        //    Log.Information("file is null");
	//        //    context.Response.StatusCode = 404;
	//        //    await context.Response.WriteAsync($"File with id {fileId} was not found.", context.RequestAborted);
	//        //    return;
	//        //}

	//        //var fileStream = await file.GetContentAsync(context.RequestAborted);
	//        //var metadata = await file.GetMetadataAsync(context.RequestAborted);

	//        //context.Response.ContentType = GetContentTypeOrDefault(metadata);
	//        //context.Response.ContentLength = fileStream.Length;

	//        //if (metadata.TryGetValue("name", out var nameMeta))
	//        //{
	//        //    context.Response.Headers.Add("Content-Disposition",
	//        //        new[] { $"attachment; filename=\"{nameMeta.GetString(Encoding.UTF8)}\"" });
	//        //}

	//        //using (fileStream)
	//        //{
	//        //    await fileStream.CopyToAsync(context.Response.Body, 81920, context.RequestAborted);
	//        //}
	//    }

	//    private static string GetContentTypeOrDefault(Dictionary<string, Metadata> metadata)
	//    {
	//        if (metadata.TryGetValue("contentType", out var contentType))
	//        {
	//            return contentType.GetString(Encoding.UTF8);
	//        }

	//        return "application/octet-stream";
	//    }
	//}

	public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		// Don't do this in production...
		private const string Username = "test";
		private const string Password = "test";

		public BasicAuthenticationHandler(
			IOptionsMonitor<AuthenticationSchemeOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			ISystemClock clock)
			: base(options, logger, encoder, clock)
		{
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			if (!Request.Headers.ContainsKey("Authorization"))
				return Task.FromResult(AuthenticateResult.NoResult());

			bool isAuthenticated;
			try
			{
				var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
				var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');
				isAuthenticated = Authenticate(credentials[0], credentials[1]);
			}
			catch
			{
				return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
			}

			if (!isAuthenticated)
				return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));

			var claims = new[] {
				new Claim(ClaimTypes.NameIdentifier, Username),
				new Claim(ClaimTypes.Name, Username),
			};

			return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name)), Scheme.Name)));
		}

		private bool Authenticate(string username, string password)
		{
			return username == Username && password == Password;
		}
	}



	public static class Helper
	{
        public static bool IsAdmin(HttpContext httpContext)
        {
			bool isAdmin = false;
			var roles = httpContext.User?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
			if (roles.Contains("Admin")) isAdmin = true;

			//var loginIdString = httpContext.User.Claims.FirstOrDefault(x => x.Type == "Admin")?.Value;

            //bool isAdmin = loginIdString is { };

            return isAdmin;
        }


        public static int? GetLoginIdAsInt(HttpContext httpContext)
		{
			var loginIdString = httpContext.User.Claims.FirstOrDefault(x => x.Type == "LoginId")?.Value;

			var result = int.TryParse(loginIdString, out int loginId);

			int? foo = null;

			if (result != false) foo = loginId;
			//if (result == false)
			//	throw new ApplicationException("Can't get loginId");

			//if (loginId == 0)
			//	throw new ApplicationException("LoginId is 0");

			return foo;
		}
	}

	public static class PasswordCreator
	{
		private static readonly char[] Punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();

		public static string Generate(int length, int numberOfNonAlphanumericCharacters)
		{
			if (length < 1 || length > 128)
			{
				throw new ArgumentException(nameof(length));
			}

			if (numberOfNonAlphanumericCharacters > length || numberOfNonAlphanumericCharacters < 0)
			{
				throw new ArgumentException(nameof(numberOfNonAlphanumericCharacters));
			}

			using (var rng = RandomNumberGenerator.Create())
			{
				var byteBuffer = new byte[length];

				rng.GetBytes(byteBuffer);

				var count = 0;
				var characterBuffer = new char[length];

				for (var iter = 0; iter < length; iter++)
				{
					var i = byteBuffer[iter] % 87;

					if (i < 10)
					{
						characterBuffer[iter] = (char)('0' + i);
					}
					else if (i < 36)
					{
						characterBuffer[iter] = (char)('A' + i - 10);
					}
					else if (i < 62)
					{
						characterBuffer[iter] = (char)('a' + i - 36);
					}
					else
					{
						characterBuffer[iter] = Punctuations[i - 62];
						count++;
					}
				}

				if (count >= numberOfNonAlphanumericCharacters)
				{
					return new string(characterBuffer);
				}

				int j;
				var rand = new Random();

				for (j = 0; j < numberOfNonAlphanumericCharacters - count; j++)
				{
					int k;
					do
					{
						k = rand.Next(0, length);
					}
					while (!char.IsLetterOrDigit(characterBuffer[k]));

					characterBuffer[k] = Punctuations[rand.Next(0, Punctuations.Length)];
				}

				return new string(characterBuffer);
			}
		}
	}
}
