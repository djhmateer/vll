using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Polly;
using Serilog;

namespace VLL.Web.Pages.project
{
	//[Authorize(Roles = "Tier1, Tier2, Admin")]
	public class editModel : PageModel
	{

		[BindProperty]
		public ProjectEditViewModel Project { get; set; } = default!;
		//public JobViewModel Job { get; set; } = null!;
		//public ProjectAllTablesViewModel ProjectAllTablesViewModel { get; set; } = null!;

		//public List<ProjectMembersViewModel> ListOfProjectMembersViewModel { get; set; } = null!;

		//public List<ProjectLinksViewModel> ListOfProjectLinksViewModel { get; set; } = null!;

		//public List<ProjectIssueViewModel> ListOfProjectIssuesViewModel { get; set; } = null!;
		//public TimeSpan TotalTime { get; set; }
		//public int QueueLength { get; set; }

		//public List<LogSmall> Logs { get; set; } = null!;

		//public string? WarningMessage { get; set; }

		//public bool ResultsFileExists { get; set; }


		//public ResultModel(FaceSearchFileProcessingChannel faceSearchFileMessageChannel, HateSpeechFileProcessingChannel hateSpeechFileProcessingChannel)
		//{
		//    _faceSearchFileMessageChannel = faceSearchFileMessageChannel;
		//    _hateSpeechFileProcessingChannel = hateSpeechFileProcessingChannel;
		//}

		public async Task<IActionResult> OnGetAsync(int projectId)
		{
			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

			var loginId = Helper.GetLoginIdAsInt(HttpContext);

					// Is this Login allowed to look at this result?
			//var isAllowed = await Db.CheckIfLoginIdIsAllowedToViewThisJobId(connectionString, loginId, jobId);

			//if (!isAllowed) return LocalRedirect("/account/access-denied");

			Project = await Db.GetProjectEditVMByProjectId(connectionString, projectId);


			return Page();
		}

		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see https://aka.ms/RazorPagesCRUD.
		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			_context.Attach(Project).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProjectExists(Project.ProjectId))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return RedirectToPage("./Index");
		}
	}
}
