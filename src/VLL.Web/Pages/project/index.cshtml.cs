using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace VLL.Web.Pages.Project
{
	//[Authorize(Roles = "Tier1, Tier2, Admin")]
	public class ProjectModel : PageModel
	{
		//public JobViewModel Job { get; set; } = null!;
		public ProjectAllTablesViewModel ProjectAllTablesViewModel { get; set; } = null!;

		public List<ProjectMembersViewModel> ListOfProjectMembersViewModel { get; set; } = null!;

		public List<ProjectLinksViewModel> ListOfProjectLinksViewModel { get; set; } = null!;

		public List<ProjectIssueViewModel> ListOfProjectIssuesViewModel { get; set; } = null!;
		//public TimeSpan TotalTime { get; set; }
		//public int QueueLength { get; set; }

		//public List<LogSmall> Logs { get; set; } = null!;

		//public string? WarningMessage { get; set; }

		//public bool ResultsFileExists { get; set; }
		public bool CanSeeEditButton { get; set; }


		//public ResultModel(FaceSearchFileProcessingChannel faceSearchFileMessageChannel, HateSpeechFileProcessingChannel hateSpeechFileProcessingChannel)
		//{
		//    _faceSearchFileMessageChannel = faceSearchFileMessageChannel;
		//    _hateSpeechFileProcessingChannel = hateSpeechFileProcessingChannel;
		//}

		public async Task<IActionResult> OnGetAsync(int projectId)
		{
			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

			var loginId = Helper.GetLoginIdAsInt(HttpContext);

			var isAdmin = Helper.IsAdmin(HttpContext);

			if (loginId == null)
			{
				// not logged in so can't see edit button
				CanSeeEditButton = false;
			}
			else
			{
				//var roles = User?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
				//if (roles.Contains("Admin"))
				if (isAdmin)
				{
					CanSeeEditButton = true;
				}
				else
				{
					// Is this loginId a Promoter of this project ie can they see the edit button?
					CanSeeEditButton = await Db.CheckIfLoginIdCanSeeEditButtonForProjectId(connectionString, (int)loginId, projectId);
				}
			}

			// Is this Login allowed to look at this result?
			//var isAllowed = await Db.CheckIfLoginIdIsAllowedToViewThisJobId(connectionString, loginId, jobId);

			//if (!isAllowed) return LocalRedirect("/account/access-denied");



			ProjectAllTablesViewModel = await Db.GetProjectByProjectId(connectionString, projectId);

			ListOfProjectMembersViewModel = await Db.GetProjectMembersByProjectId(connectionString, projectId);

			ListOfProjectLinksViewModel = await Db.GetLinksByProjectId(connectionString, projectId);

			var getPrivateIssues = CanSeeEditButton;
			ListOfProjectIssuesViewModel = await Db.GetIssuesByProjectId(connectionString, projectId, getPrivateIssues);

		
			return Page();
		}
	}
}
