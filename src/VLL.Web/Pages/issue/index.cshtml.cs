using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace VLL.Web.Pages.issue
{
    //[Authorize(Roles = "Tier1, Tier2, Admin")]
    public class IssueModel : PageModel
    {
        //public JobViewModel Job { get; set; } = null!;
        public IssueAllTablesViewModel IssueAllTablesViewModel { get; set; } = null!;

        public bool CanSeeEditButton { get; set; }

        public async Task<IActionResult> OnGetAsync(int issueId)
        {
            var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

            var loginId = Helper.GetLoginIdAsInt(HttpContext);

            IssueAllTablesViewModel = await Db.GetIssueByIssueId(connectionString, issueId);

            if (loginId == null)
            {
                // not logged in so can't see edit button
                CanSeeEditButton = false;
            }
            else
            {
                // is this login an admin?
                var isAdmin = Helper.IsAdmin(HttpContext);

                //var roles = User?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                //if (roles.Contains("Admin"))
                if (isAdmin)
                {
                    CanSeeEditButton = true;
                }
                else
                {
                    // Is this loginId a Promoter of this project ie can they see the edit button?
                    CanSeeEditButton = await Db.CheckIfLoginIdCanSeeEditButtonForProjectId(connectionString, (int)loginId,
                        IssueAllTablesViewModel.ProjectId);
                }
            }


            //var loginId = Helper.GetLoginIdAsInt(HttpContext);

            // Is this Login allowed to look at this result?
            //var isAllowed = await Db.CheckIfLoginIdIsAllowedToViewThisJobId(connectionString, loginId, jobId);

            //if (!isAllowed) return LocalRedirect("/account/access-denied");

            //ProjectAllTablesViewModel = await Db.GetProjectByProjectId(connectionString, projectId);

            //ListOfProjectMembersViewModel = await Db.GetProjectMembersByProjectId(connectionString, projectId);

            //ListOfProjectLinksViewModel = await Db.GetLinksByProjectId(connectionString, projectId);

            //ListOfProjectIssuesViewModel = await Db.GetIssuesByProjectId(connectionString, projectId);

            //string? jobStatusString = job.JobStatusId switch
            //{
            //    Db.JobStatusId.WaitingToStart => "Waiting to Start",
            //    Db.JobStatusId.Running => "Running",
            //    Db.JobStatusId.Completed => "Completed",
            //    Db.JobStatusId.CancelledByUser => "Cancelled by User",
            //    Db.JobStatusId.Exception => "Exception",
            //    _ => null
            //};

            //string? jobType = null;
            //if (job.JobTypeId == Db.JobTypeId.FaceSearch) jobType = "FaceSearch";
            //if (job.JobTypeId == Db.JobTypeId.HateSpeech) jobType = "HateSpeech";

            //var jvm = new JobViewModel(job.JobId, job.LoginId, job.OrigFileName, job.DateTimeUtcUploaded,
            //    job.JobStatusId, jobStatusString, job.VMId, job.DateTimeUtcJobStartedOnVm,
            //    job.DateTimeUtcJobEndedOnVm, job.JobTypeId, jobType);

            //Job = jvm;


            //var logs = await Db.GetLogsForJobId(connectionString, jobId);
            //Logs = logs;

            //TimeSpan? totalTime = job.DateTimeUtcJobEndedOnVm - job.DateTimeUtcJobStartedOnVm;
            //if (totalTime != null)
            //    TotalTime = ((TimeSpan)totalTime)!;
            //else
            //{
            //    if (job.DateTimeUtcJobStartedOnVm is { })
            //    {
            //        // If the job has started but hasn't completed yet
            //        TotalTime = ((TimeSpan)(DateTime.UtcNow - job.DateTimeUtcJobStartedOnVm))!;
            //    }
            //    else
            //    {
            //        // Job hasn't started
            //        TotalTime = TimeSpan.Zero;
            //    }
            //}

            //if (job.JobTypeId == Db.JobTypeId.FaceSearch) QueueLength = _faceSearchFileMessageChannel.CountOfFileProcessingChannel();
            //if (job.JobTypeId == Db.JobTypeId.HateSpeech) QueueLength = _hateSpeechFileProcessingChannel.CountOfFileProcessingChannel();

            //// unusual situation - well not really. Timing issue? 
            //// do prompt the user
            //if (job.JobStatusId == Db.JobStatusId.WaitingToStart && QueueLength == 0)
            //{
            //    WarningMessage = "Please refresh this page.";

            //    Log.Information("Please refresh this page as it looks like the job is waiting to start, and is not in the queue. If this continues, please try submitting the job again, and otherwise contact us.");
            //}

            //// does the results file exist?
            //// which means is the length > 0 bytes, as we're not gracefully deleting 0 bytes on creation
            //// if it doesn't it can mean there was a problem on the remote VM eg HateSpeech csv parser failed
            //ResultsFileExists = false;
            //try
            //{
            //    var pathLocalDestinationDirectory = Path.Combine("/mnt/osrshare/", $"downloads/{jobId}");
            //    var htmlFileName = "results.html";
            //    var pathLocalFile = Path.Combine(pathLocalDestinationDirectory, htmlFileName);
            //    if (System.IO.File.Exists(pathLocalFile))
            //    {
            //        var file = new FileInfo(pathLocalFile);

            //        ResultsFileExists = file.Length > 0;
            //    }
            //    else
            //        ResultsFileExists = false;
            //}
            //catch (Exception ex)
            //{
            //    Log.Warning(ex,"ResultsFileExists trapped and swallowed exception - should never happen");
            //}

            return Page();
        }
    }
}
