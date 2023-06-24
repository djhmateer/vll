using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Polly;
using Serilog;

namespace VLL.Web.Pages.link
{
	[Authorize(Roles = "Tier1, Tier2, Admin")]
	public class DeleteModel : PageModel
	{
		[BindProperty]
		public ProjectLinksViewModel Link { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(int linkId)
		{
			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

			var loginId = Helper.GetLoginIdAsInt(HttpContext);

			Link = await Db.GetLinkByLinkId(connectionString, linkId);

			// is current user an admin?
			bool isAdmin = Helper.IsAdmin(HttpContext);

			if (isAdmin) { }
			else
			{
				// Is this Login allowed to view the delete screen of this link 
				// ie are they a promoter of the related project?
				var isAllowed = await Db.CheckIfLoginIdIsAllowedToEditThisProject(connectionString, loginId, Link.ProjectId);
				if (!isAllowed) return LocalRedirect("/account/access-denied");
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{

			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;
			var loginId = Helper.GetLoginIdAsInt(HttpContext);
			var p = Link;

			bool isAdmin = Helper.IsAdmin(HttpContext);

			if (isAdmin) { }
			else
			{
				// Is this Login allowed to view the edit screen of this issue
				// ie are they a promoter of the related project?
				var isAllowed = await Db.CheckIfLoginIdIsAllowedToEditThisProject(connectionString, loginId, p.ProjectId);
				if (!isAllowed) return LocalRedirect("/account/access-denied");
			}

			await Db.DeleteLinkByLinkId(connectionString, p.LinkId);

			return Redirect($"/project/{p.ProjectId}");
		}
	}
}
