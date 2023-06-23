using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace VLL.Web.Pages
{
    public class zOLDIndexModel : PageModel
    {
        public List<ProjectViewModel> Challenges { get; set; } = null!;
        public List<ProjectViewModel> Ongoing { get; set; } = null!;
        public List<ProjectViewModel> Completed { get; set; } = null!;

        public List<IssueViewModel> Issues { get; set; } = null!;

        public async Task OnGet()
        {

            var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

            //var challenges = await Db.GetAllChallengeProjects(connectionString);

            Challenges = await Db.GetAllPublicChallengeProjects(connectionString);
            Ongoing = await Db.GetAllPublicOngoingProjects(connectionString);
            Completed = await Db.GetAllPublicCompletedProjects(connectionString);

            Issues = await Db.GetAllIssues(connectionString);
        }
    }
}