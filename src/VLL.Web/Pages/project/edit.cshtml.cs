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
	//[Authorize(Roles = "Tier1, Tier2, Admin")]
	public class editModel : PageModel
	{

		[BindProperty]
		public ProjectEditViewModel Project { get; set; } = default!;

		[BindProperty]
		public int SelectedProjectStatusId { get; set; }
		public List<SelectListItem> ProjectStatusOptions { get; set; } = null!;

		public async Task<IActionResult> OnGetAsync(int projectId)
		{
			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

			var loginId = Helper.GetLoginIdAsInt(HttpContext);

			// Is this Login allowed to edit this project?
			//var isAllowed = await Db.CheckIfLoginIdIsAllowedToViewThisJobId(connectionString, loginId, jobId);

			//if (!isAllowed) return LocalRedirect("/account/access-denied");

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



			return Page();
		}

		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see https://aka.ms/RazorPagesCRUD.
		//public async Task<IActionResult> OnPostAsync(ProjectEditViewModel p)
		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				Log.Information("Modelstate not valid");
				return Page();
			}

			Log.Information("Saving!");
			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;


			var p = Project;
			await Db.UpdateProjectByProjectId(connectionString, p.ProjectId, p.Name, SelectedProjectStatusId, p.IsPublic,
				p.PromoterLoginId, p.ShortDescription, p.Description, p.Keywords, p.DateTimeCreatedUtc, p.ResearchNotes);

			//try
			//{
			//	await _context.SaveChangesAsync();
			//}
			//catch (DbUpdateConcurrencyException)
			//{
			//	if (!ProjectExists(Project.ProjectId))
			//	{
			//		return NotFound();
			//	}
			//	else
			//	{
			//		throw;
			//	}
			//}

			return RedirectToPage("/projects");
		}
	}
}
