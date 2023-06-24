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
	public class addModel : PageModel
	{
		[BindProperty]
		public int SelectedLoginId { get; set; }
		public List<SelectListItem> LoginOptions { get; set; } = null!;

		[BindProperty]
		public int ProjectId { get; set; }

		public async Task<IActionResult> OnGetAsync(int projectId)
		{
			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

			var loginId = Helper.GetLoginIdAsInt(HttpContext);

			bool isAdmin = Helper.IsAdmin(HttpContext);

			if (isAdmin) { }
			else
			{
				// Is this Login allowed to add members to project? 
				// ie are they a promoter of the related project?
				var isAllowed = await Db.CheckIfLoginIdIsAllowedToEditThisProject(connectionString, 
					loginId, projectId);
				if (!isAllowed) return LocalRedirect("/account/access-denied");
			}

			var p = await Db.GetLoginsNotInProject(connectionString, projectId);

			LoginOptions = p.Select(x =>
				new SelectListItem
				{
					Value = x.LoginId.ToString(),
					Text = x.Email
				}).ToList();
			//SelectedIssueStatusId = Issue.IssueStatusId;


			//ProjectName = p.Name;
			//ProjectId = projectId;

			return Page();
		}

		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see https://aka.ms/RazorPagesCRUD.
		//public async Task<IActionResult> OnPostAsync(ProjectEditViewModel p)
		public async Task<IActionResult> OnPostAsync(int projectId)
		{
			//if (!ModelState.IsValid)
			//{
			//	return Page();
			//}

			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;
			var loginId = Helper.GetLoginIdAsInt(HttpContext);

			bool isAdmin = Helper.IsAdmin(HttpContext);

			if (isAdmin) { }
			else
			{
				// Is this Login allowed to add member? 
				// ie are they a promoter of the related project?
				var isAllowed = await Db.CheckIfLoginIdIsAllowedToEditThisProject(connectionString, loginId, projectId);
				if (!isAllowed) return LocalRedirect("/account/access-denied");
			}

			await Db.AddLoginIdToProjectLogin(connectionString, SelectedLoginId, projectId);

			return Redirect($"/project/{projectId}");
		}
	}
}
