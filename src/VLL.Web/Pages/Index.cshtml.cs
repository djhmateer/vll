using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace VLL.Web.Pages
{
    public class IndexModel : PageModel
    {
        public List<ProjectViewModel> Challenges { get; set; } = null!;
        public List<ProjectViewModel> Ongoing { get; set; } = null!;
        public List<ProjectViewModel> Completed { get; set; } = null!;

        public List<IssueViewModel> Issues { get; set; } = null!;

        public string? CacheBust { get; set; }

        public async Task OnGet()
        {
            var base64Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            CacheBust = base64Guid;

            var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

            //var challenges = await Db.GetAllChallengeProjects(connectionString);

            Challenges = await Db.GetAllPublicChallengeProjects(connectionString);
            Ongoing = await Db.GetAllPublicOngoingProjects(connectionString);
            Completed = await Db.GetAllPublicCompletedProjects(connectionString);

            Issues = await Db.GetAllPublicIssues(connectionString);
        }
    }
}