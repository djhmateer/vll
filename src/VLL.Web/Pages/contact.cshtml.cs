﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VLL.Web.Pages
{
    public class ContactModel : PageModel
    {
        public string? CacheBust { get; set; }

        public void OnGet()
        {
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPost(string? email, string? message)
        {
            // could do a lot of modelstate validation here
            if (string.IsNullOrEmpty(email))
                return LocalRedirect("/email-fail");

            var postmarkServerToken = AppConfiguration.LoadFromEnvironment().PostmarkServerToken;

            //var response = await Email.SendTemplate("forgot-password", email, "Thank you - we will get back to you!", postmarkServerToken);

            // send confirmation to the person who just submitted email on site
            {
                var ToEmailAddress = email;
                var subject = "Thank you";
                var text = "Thank you - we will get back to you soon";
                var html = "Thank you - we will get back to you soon";
                var foo = new OSREmail(ToEmailAddress, subject, text, html);
                var response = await Email.Send(foo, postmarkServerToken, null);
            }

            // send email to the admin 
            {
                var ToEmailAddress = "dave@hmsoftware.co.uk";
                var subject = "auto-archiver message";
                var text = $"sender is: {email}, message is: {message}";
                var html = $"sender is: {email}, message is: {message}";
                var foo = new OSREmail(ToEmailAddress, subject, text, html);
                var response = await Email.Send(foo, postmarkServerToken, null);
            }

            // PRG
            return LocalRedirect("/email-success");
        }
    }
}