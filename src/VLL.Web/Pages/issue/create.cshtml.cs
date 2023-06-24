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
	public class createModel : PageModel
	{

		[BindProperty]
		public IssueEditViewModel Issue { get; set; } = default!;

		[BindProperty]
		public int SelectedIssueStatusId { get; set; }
		public List<SelectListItem> IssueStatusOptions { get; set; } = null!;

		[BindProperty]
		public int? SelectedRegulatorId { get; set; }
		public List<SelectListItem> RegulatorOptions { get; set; } = null!;

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
				// Is this Login allowed to add an issue?
				// ie are they a promoter of the related project?
				var isAllowed = await Db.CheckIfLoginIdIsAllowedToEditThisProject(connectionString, 
					loginId, projectId);
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
			//SelectedIssueStatusId = Issue.IssueStatusId;

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

			var bar = await Db.GetProjectByProjectId(connectionString, projectId);
			ProjectName = bar.Name;
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
			var p = Issue;

			bool isAdmin = Helper.IsAdmin(HttpContext);

			if (isAdmin) { }
			else
			{
				// Is this Login allowed to view the edit screen of this issue
				// ie are they a promoter of the related project?
				var isAllowed = await Db.CheckIfLoginIdIsAllowedToEditThisProject(connectionString, loginId, Issue.ProjectId);
				if (!isAllowed) return LocalRedirect("/account/access-denied");
			}

			// regulator ddl
			int? foo = null;
			if (SelectedRegulatorId == 0) { }
			else
			{
				foo = (int)SelectedRegulatorId;
			}

			//await Db.UpdateIssueByIssueId(connectionString, Issue, SelectedIssueStatusId, foo);
			var issueId = await Db.CreateIssueAndReturnIssueId(connectionString, Issue, SelectedIssueStatusId, foo, projectId);

			return Redirect($"/issue/{issueId}");
		}
	}
}
