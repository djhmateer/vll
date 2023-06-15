using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace VLL.Web.Pages
{
    public class IndexModel : PageModel
    {
        public List<ProjectViewModel> Projects { get; set; } = null!;

        public async Task OnGet()
        {

            var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;
            var projects = await Db.GetAllChallengeProjects(connectionString);
            //var projectViewModels = new List<ProjectViewModel>();

            Projects = projects;
        }
    }
}