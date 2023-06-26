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

namespace VLL.Web.Pages.member
{
	[Authorize(Roles = "Tier1, Tier2, Admin")]
	public class DeleteModel : PageModel
	{
		[BindProperty]
		public ProjectMembersViewModel ProjetLoginViewModel { get; set; } = default!;

		[BindProperty]
		public int ProjectId { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(int projectId, int loginId)
		{
			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

			ProjetLoginViewModel = await Db.GetLoginByProjectIdAndLoginId(connectionString, projectId, loginId);

			// is current user an admin?
			bool isAdmin = Helper.IsAdmin(HttpContext);

			if (isAdmin) { }
			else
			{
				// Is this Login allowed to view the delete screen of this link 
				// ie are they a promoter of the related project?
				var currentLoginId = Helper.GetLoginIdAsInt(HttpContext);
				var isAllowed = await Db.CheckIfLoginIdIsAllowedToEditThisProject(connectionString, currentLoginId, projectId);
				if (!isAllowed) return LocalRedirect("/account/access-denied");
			}
			ProjectId = projectId;

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

			bool isAdmin = Helper.IsAdmin(HttpContext);

			if (isAdmin) { }
			else
			{
				// Is this Login allowed to view the delete screen of this link 
				// ie are they a promoter of the related project?
				var currentLoginId = Helper.GetLoginIdAsInt(HttpContext);
				var isAllowed = await Db.CheckIfLoginIdIsAllowedToEditThisProject(connectionString, currentLoginId, ProjectId);
				if (!isAllowed) return LocalRedirect("/account/access-denied");
			}

			var p = ProjetLoginViewModel;
			await Db.DeleteProjectLoginByProjectIdAndLoginId(connectionString, ProjectId, p.LoginId);

			return Redirect($"/project/{ProjectId}");
		}
	}
}
