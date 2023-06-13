using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace VLL.Web.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            Log.Information("hello");

        }
    }
}