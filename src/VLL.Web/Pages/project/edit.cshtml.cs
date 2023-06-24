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

namespace VLL.Web.Pages.project
{
	[Authorize(Roles = "Tier1, Tier2, Admin")]
	public class editModel : PageModel
	{

		[BindProperty]
		public ProjectEditViewModel Project { get; set; } = default!;

		[BindProperty]
		public int SelectedProjectStatusId { get; set; }
		public List<SelectListItem> ProjectStatusOptions { get; set; } = null!;

		[BindProperty]
		public int? SelectedPromoterLoginId { get; set; }
		public List<SelectListItem> PromoterLoginOptions { get; set; } = null!;


		public async Task<IActionResult> OnGetAsync(int projectId)
		{
			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

			var loginId = Helper.GetLoginIdAsInt(HttpContext);

			// is current user an admin?
			bool isAdmin = Helper.IsAdmin(HttpContext);

			if (isAdmin) { }
			else
			{
				// Is this Login allowed to view the edit screen of this project?
				var isAllowed = await Db.CheckIfLoginIdIsAllowedToEditThisProject(connectionString, loginId, projectId);
				if (!isAllowed) return LocalRedirect("/account/access-denied");
			}

			Project = await Db.GetProjectEditVMByProjectId(connectionString, projectId);

			// ddl for projectStatusId
			var projectStatuses = await Db.GetAllProjectStatuses(connectionString);
			ProjectStatusOptions = projectStatuses.Select(x =>
				new SelectListItem
				{
					Value = x.ProjectStatusId.ToString(),
					Text = x.Name
				}).ToList();
			SelectedProjectStatusId = Project.ProjectStatusId;

			// ddl for promoterLoginId
			// which may be none
			//var promoterLogins = await Db.GetAllPromoterLogins(connectionString);
			//PromoterLoginOptions = promoterLogins.Select(x =>
			//	new SelectListItem
			//	{
			//		Value = x.LoginId.ToString(),
			//		Text = x.Email
			//	}).ToList();
			PromoterLoginOptions = new List<SelectListItem>
			{
				new SelectListItem("none","0")
			};
			var promoterLogins = await Db.GetAllPromoterLogins(connectionString);
			foreach (var p in promoterLogins)
			{
				var foo = new SelectListItem
				{
					Value = p.LoginId.ToString(),
					Text = p.Email
				};
				PromoterLoginOptions.Add(foo);
			}
			SelectedPromoterLoginId = Project.PromoterLoginId;

			return Page();
		}

		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see https://aka.ms/RazorPagesCRUD.
		//public async Task<IActionResult> OnPostAsync(ProjectEditViewModel p)
		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;
			var loginId = Helper.GetLoginIdAsInt(HttpContext);
			var p = Project;

			bool isAdmin = Helper.IsAdmin(HttpContext);

			if (isAdmin) { }
			else
			{
				// Is current Login allowed to update the this project
				var isAllowed = await Db.CheckIfLoginIdIsAllowedToEditThisProject(connectionString, loginId, p.ProjectId);
				if (!isAllowed) return LocalRedirect("/account/access-denied");
			}

			int? foo = null;
			if (SelectedPromoterLoginId == 0) { }
			else
			{
				foo = (int)SelectedPromoterLoginId;
			}

			await Db.UpdateProjectByProjectId(connectionString, p.ProjectId, p.Name, SelectedProjectStatusId, p.IsPublic,
			foo, p.ShortDescription, p.Description, p.Keywords, p.DateTimeCreatedUtc, p.ResearchNotes);

			return Redirect($"/project/{p.ProjectId}");
		}
	}
}
