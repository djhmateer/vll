using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PostmarkDotNet;
using Serilog;

namespace VLL.Web.Pages.account
{
	[Authorize]
	public class ManageLoginModel : PageModel
	{
		[BindProperty]
		public LogindAndPerson LoginAndPerson { get; set; } = null!;

		public bool DisplaySuccessSave { get; set; }


		public IHttpContextAccessor HttpContextAccessor { get; }

		public string Email { get; set; } = null!;

		public string RoleLevel { get; set; } = null!;

		public ManageLoginModel(IHttpContextAccessor httpContextAccessor) => HttpContextAccessor = httpContextAccessor;

		public async Task<IActionResult> OnGet(string? displaySuccessSave = "false")
		{
			Email = User.Identity!.Name!;

			var roleClaims = User.FindAll(ClaimTypes.Role);

			// Login will probably only have 1 Claim ie Tier1, Tier2, Admin
			foreach (var claim in roleClaims)
			{
				RoleLevel += claim.Value + " ";
			}

			// is loginId stored in cookie?
			var loginId = Helper.GetLoginIdAsInt(HttpContext);

			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

			LoginAndPerson = await Db.GetLoginAndPersonByLoginId(connectionString, loginId);

			if (displaySuccessSave == "true")
				DisplaySuccessSave = true;

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;
			var currentLoginId = Helper.GetLoginIdAsInt(HttpContext);

			if (currentLoginId == LoginAndPerson.LoginId) { }
			else return LocalRedirect("/account/access-denied");

			await Db.UpdateLoginAndPersonByLoginId(connectionString, LoginAndPerson);

			//DisplaySuccessSave = true;

			return Redirect($"/account/manage?DisplaySuccessSave=true");
		}
	}
}


