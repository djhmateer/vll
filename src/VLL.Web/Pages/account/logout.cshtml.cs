﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace VLL.Web.Pages.account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            Log.Warning("Should not hit this logout page directly. Should use a post ");
            return LocalRedirect("/error");
        }

        public async Task<IActionResult> OnPost()
        {
            Log.Information($"User {User?.Identity?.Name} logged out");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Does a 302Found GET request to the current page
            //return RedirectToPage();
            return RedirectToPage("/account/logout-success");
        }
    }
}


