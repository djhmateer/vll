using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Polly;
using Serilog;

namespace VLL.Web.Pages.project
{
	[Authorize(Roles = "Admin")]
	public class createModel : PageModel
	{
		[BindProperty]
		public ProjectEditViewModel Project { get; set; } = default!;

		[BindProperty]
		public int SelectedProjectStatusId { get; set; }
		public List<SelectListItem> ProjectStatusOptions { get; set; } = null!;

		[BindProperty]
		public int? SelectedPromoterLoginId { get; set; }
		public List<SelectListItem> PromoterLoginOptions { get; set; } = null!;


		public async Task<IActionResult> OnGetAsync()
		{
			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

			var loginId = Helper.GetLoginIdAsInt(HttpContext);

			// ddl for projectStatusId
			var projectStatuses = await Db.GetAllProjectStatuses(connectionString);
			ProjectStatusOptions = projectStatuses.Select(x =>
				new SelectListItem
				{
					Value = x.ProjectStatusId.ToString(),
					Text = x.Name
				}).ToList();

			// ddl for promoterLoginId with added none
			// not sure why null didn't work here, so special case of 0
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

			return Page();
		}

		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see https://aka.ms/RazorPagesCRUD.
		//public async Task<IActionResult> OnPostAsync(ProjectEditViewModel p)
		public async Task<IActionResult> OnPostAsync()
		{
			// as is a new project
			ModelState.Remove("ProjectId");

			if (!ModelState.IsValid)
			{
				return Page();
			}

			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;
			var loginId = Helper.GetLoginIdAsInt(HttpContext);
			ProjectEditViewModel p = Project;

			int? foo = null;
			if (SelectedPromoterLoginId == 0) { } else
			{
				foo = (int)SelectedPromoterLoginId;
			}

			var projectId = await Db.CreateProjectAndReturnProjectId(connectionString, p.ProjectId, p.Name, SelectedProjectStatusId, p.IsPublic,
			foo, p.ShortDescription, p.Description, p.Keywords, p.DateTimeCreatedUtc, p.ResearchNotes);

			return Redirect($"/project/{projectId}");
		}
	}
}
