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
	public class createModel : PageModel
	{

		[BindProperty]
		public ProjectLinksViewModel Link { get; set; } = default!;

		public string ProjectName { get; set; } = default!;
		public int ProjectId { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(int projectId)
		{
			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

			var loginId = Helper.GetLoginIdAsInt(HttpContext);

			bool isAdmin = Helper.IsAdmin(HttpContext);

			if (isAdmin) { }
			else
			{
				// Is this Login allowed to add a link?
				// ie are they a promoter of the related project?
				var isAllowed = await Db.CheckIfLoginIdIsAllowedToEditThisProject(connectionString, 
					loginId, projectId);
				if (!isAllowed) return LocalRedirect("/account/access-denied");
			}

			var p = await Db.GetProjectByProjectId(connectionString, projectId);
			ProjectName = p.Name;
			ProjectId = projectId;

			return Page();
		}

		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see https://aka.ms/RazorPagesCRUD.
		//public async Task<IActionResult> OnPostAsync(ProjectEditViewModel p)
		public async Task<IActionResult> OnPostAsync(int projectId)
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;
			var loginId = Helper.GetLoginIdAsInt(HttpContext);
			var p = Link;

			bool isAdmin = Helper.IsAdmin(HttpContext);

			if (isAdmin) { }
			else
			{
				// Is this Login allowed to add a link? 
				// ie are they a promoter of the related project?
				var isAllowed = await Db.CheckIfLoginIdIsAllowedToEditThisProject(connectionString, loginId, projectId);
				if (!isAllowed) return LocalRedirect("/account/access-denied");
			}

			var linkId = await Db.CreateLinkAndReturnLinkId(connectionString, Link, projectId);

			return Redirect($"/project/{projectId}");
		}
	}
}
