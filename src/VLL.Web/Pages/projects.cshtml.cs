using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace VLL.Web.Pages
{
	//[Authorize]
	public class ProjectsModel : PageModel
	{
		//private readonly FaceSearchFileProcessingChannel _faceSearchFileMessageChannel;
		//private readonly HateSpeechFileProcessingChannel _hateSpeechFileProcessingChannel;
		//private readonly SpeechPartsFileProcessingChannel _speechPartsFileProcessingChannel;

		//public int JobId { get; set; }

		public List<ProjectFullViewModel> Projects { get; set; } = null!;

		public string PageTitle { get; set; } = null!;

		// from the Microsoft.AspNetCore.Mvc namespace
		[FromQuery(Name = "status")]
		public string? Status { get; set; }

		//public int FaceSearchQueueLength { get; set; }
		//public int HateSpeechQueueLength { get; set; }
		//public int SpeechPartsQueueLength { get; set; }

		//public ResultsModel(FaceSearchFileProcessingChannel faceSearchFileMessageChannel, HateSpeechFileProcessingChannel hateSpeechFileProcessingChannel, SpeechPartsFileProcessingChannel speechPartsFileProcessingChannel)
		//{
		//    _faceSearchFileMessageChannel = faceSearchFileMessageChannel;
		//    _hateSpeechFileProcessingChannel = hateSpeechFileProcessingChannel;
		//    _speechPartsFileProcessingChannel = speechPartsFileProcessingChannel;
		//}

		public async Task<IActionResult> OnGetAsync()
		{
			Log.Information($"status is {Status}");

			//var loginId = Helper.GetLoginIdAsInt(HttpContext);

			var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

			int? statusId = null;
			if (Status == "challenge")
			{
				statusId = 1;
				PageTitle = "All Challenges";
			}

			if (Status == "ongoing")
			{
				statusId = 2;
				PageTitle = "All Ongoing Projects";
			}

			if (Status == "completed")
			{
				statusId = 3;
				PageTitle = "All Completed Projects";
			}

			var isAdmin = Helper.IsAdmin(HttpContext);
			if (isAdmin)
			{
				var projects = await Db.GetAllProjects(connectionString, statusId);
				Projects = projects;
			}
			else
			{
				var projects = await Db.GetAllPublicProjects(connectionString, statusId);
				Projects = projects;
			}

			//var projects = new List<ProjectFullViewModel>();
			//foreach (var p in projects)
			//{
			//    string? jobStatusString = null;

			//    if (job.JobStatusId == Db.JobStatusId.WaitingToStart) jobStatusString = "Waiting to Start";
			//    if (job.JobStatusId == Db.JobStatusId.Running) jobStatusString = "Running";
			//    if (job.JobStatusId == Db.JobStatusId.Completed) jobStatusString = "Completed";
			//    if (job.JobStatusId == Db.JobStatusId.CancelledByUser) jobStatusString = "Cancelled by User";
			//    if (job.JobStatusId == Db.JobStatusId.Exception) jobStatusString = "Exception";

			//    string? jobType = null;
			//    if (job.JobTypeId == Db.JobTypeId.FaceSearch) jobType = "FaceSearch";
			//    if (job.JobTypeId == Db.JobTypeId.HateSpeech) jobType = "HateSpeech";
			//    if (job.JobTypeId == Db.JobTypeId.SpeechParts) jobType = "SpeechParts";

			//    var jvm = new JobViewModel(job.JobId, job.LoginId, job.OrigFileName, job.DateTimeUtcUploaded,
			//        job.JobStatusId, jobStatusString, job.VMId, job.DateTimeUtcJobStartedOnVm,
			//        job.DateTimeUtcJobEndedOnVm, job.JobTypeId, jobType);

			//    listOfJobViewModels.Add(jvm);
			//}
			//Jobs = listOfJobViewModels;

			return Page();
		}
	}
}
