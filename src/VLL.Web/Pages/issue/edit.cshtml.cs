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

namespace VLL.Web.Pages.issue
{
	[Authorize(Roles = "Tier1, Tier2, Admin")]
	public class editModel : PageModel
	{

		[BindProperty]
		public IssueEditViewModel Issue { get; set; } = default!;

		[BindProperty]
		public int SelectedIssueStatusId { get; set; }
		public List<SelectListItem> IssueStatusOptions { get; set; } = null!;

		[BindProperty]
		public int? SelectedRegulatorId { get; set; }
		public List<SelectListItem> RegulatorOptions { get; set; } = null!;


		public async Task<IActionResult> OnGetAsync(int issueId)
		{
			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

			var loginId = Helper.GetLoginIdAsInt(HttpContext);

			Issue = await Db.GetIssueEditVMByIssueId(connectionString, issueId);

			// is current user an admin?
			bool isAdmin = Helper.IsAdmin(HttpContext);

			if (isAdmin) { }
			else
			{
				// Is this Login allowed to view the edit screen of this issue
				// ie are they a promoter of the related project?
				var isAllowed = await Db.CheckIfLoginIdIsAllowedToEditThisProject(connectionString, loginId, Issue.ProjectId);
				if (!isAllowed) return LocalRedirect("/account/access-denied");
			}


			// issue status ddl 
			var issueStatuses = await Db.GetAllIssueStatuses(connectionString);
			IssueStatusOptions = issueStatuses.Select(x =>
				new SelectListItem
				{
					Value = x.IssueStatusId.ToString(),
					Text = x.Name
				}).ToList();
			SelectedIssueStatusId = Issue.IssueStatusId;

			// regulator ddl - can be null

			RegulatorOptions = new List<SelectListItem>
			{
				new SelectListItem("none","0")
			};
			var regulators = await Db.GetAllRegulators(connectionString);
			foreach (var r in regulators)
			{
				var foo = new SelectListItem
				{
					Value = r.RegulatorId.ToString(),
					Text = r.Name
				};
				RegulatorOptions.Add(foo);
			}
			SelectedRegulatorId = Issue.RegulatorId;

			return Page();
		}

		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see https://aka.ms/RazorPagesCRUD.
		//public async Task<IActionResult> OnPostAsync(ProjectEditViewModel p)
		public async Task<IActionResult> OnPostAsync()
		{
			//if (!ModelState.IsValid)
			//{
			//	return Page();
			//}

			//var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;
			//var loginId = Helper.GetLoginIdAsInt(HttpContext);
			//var p = Project;

			//bool isAdmin = Helper.IsAdmin(HttpContext);

			//if (isAdmin) { }
			//else
			//{
			//	// Is current Login allowed to update the this project
			//	var isAllowed = await Db.CheckIfLoginIdIsAllowedToEditThisProject(connectionString, loginId, p.ProjectId);
			//	if (!isAllowed) return LocalRedirect("/account/access-denied");
			//}

			//int? foo = null;
			//if (SelectedPromoterLoginId == 0) { }
			//else
			//{
			//	foo = (int)SelectedPromoterLoginId;
			//}

			//await Db.UpdateProjectByProjectId(connectionString, p.ProjectId, p.Name, SelectedProjectStatusId, p.IsPublic,
			//foo, p.ShortDescription, p.Description, p.Keywords, p.DateTimeCreatedUtc, p.ResearchNotes);

			//return Redirect($"/project/{p.ProjectId}");
			return RedirectToPage($"/project/4");
		}
	}
}
