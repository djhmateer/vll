﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PostmarkDotNet;
using Serilog;

namespace VLL.Web.Pages.account
{
    public class RegisterModel : PageModel
    {
        public IHttpContextAccessor HttpContextAccessor { get; }

        [BindProperty]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [BindProperty]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Must be at least {2} characters long, and 1 capital letter", MinimumLength = 8)]
        //[StringLength(100, ErrorMessage = "Must be at least {2} characters long", MinimumLength = 1)]
        public string Password { get; set; } = null!;


        public RegisterModel(IHttpContextAccessor httpContextAccessor) => HttpContextAccessor = httpContextAccessor;

        public IActionResult OnGet()
        {
            // Has a logged in user somehow got to this Register page?
            if (User.Identity is { IsAuthenticated: true }) return LocalRedirect("/");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;

            //ReturnUrl = returnUrl;
            if (Password.Any(char.IsUpper) != true)
                ModelState.AddModelError("Password", "At least 1 capital letter");

            if (ModelState.IsValid)
            {
                // Check if the email exists in db?
                var result = await Db.GetLoginByEmail(connectionString, Email);

                if (result is { })
                {
                    if (result.LoginStateId == LoginStateId.WaitingToBeInitiallyVerifiedByEmail)
                    {
                        // User is registering again with an unverified email - this is fine.
                        await Db.DeleteLoginWithEmail(connectionString, Email);
                    }
                    else
                    {
                        Log.Information($@"User tried to register an already registered email address {Email}");
                        ModelState.AddModelError("Email", "Sorry email address is already registered - try logging in or resetting password");
                        return Page();
                    }
                }

                // Insert new login in the database
                // we will allow upper and lower cases in email addresses
                // https://webmasters.stackexchange.com/questions/34056/is-it-ok-to-use-uppercase-letters-in-an-email-address/34058
                // but will not allow duplicate accounts with UpperAndLower
                var login = new LoginSmall
                (
                    // Will be assigned by the Db
                    LoginId: null,
                    Email,
                    Password.HashPassword(),
                    LoginStateId.WaitingToBeInitiallyVerifiedByEmail,
                    // RoleId is null until LoginStateId past WaitingToBeInitiallyVerifiedByEmail ie 2 - InUse
                    // which is set in /account/email-address-confirmation
                    RoleId: null
                );

                var returnedLogin = await Db.InsertLogin(connectionString, login);

                // Generate EmailAddressConfirmationCode which will be checked by /account/email-address-confirmation
                var guid = Guid.NewGuid();
                await Db.UpdateLoginEmailAddressConfirmationCode(connectionString, returnedLogin.LoginId, guid);

                // shortcut way to get the url eg https://localhost500 or https://testserver.azure.com
                // to make testing easier
                var request = HttpContextAccessor.HttpContext?.Request;

                // eg https
                // we are using a reverse proxy, which is communicating over http
                // so this will always give http
                //var scheme = request?.Scheme;

                // we are forcing redirect to https on nginx, so can safely specify http here
                var scheme = "https";

                // eg localhost:5001
                var host = request?.Host.ToUriComponent();

                // old email stuff before template
//                var foo = $"{scheme}://" + host + $"/account/email-address-confirmation/{guid}";
//                Log.Information(foo);

//                var textBody = $@"Hi,
//Here is your OSR4Rights Tools email address confirmation link: {foo}
//Please click this link within 1 hour from now
//Or register again if you miss this time
//";

//                var htmlText = $@"<p>Hi,</p>
//<p>Here is your OSR4Rights Tools email address confirmation link: </p>
//<p><a href=""{foo}"">{foo}</a></p>
//<p>Please click this link within 1 hour from now</p>
//<p>Or register again if you miss this time</p>
//                    ";

//                var osrEmail = new OSREmail(
//                    ToEmailAddress: Email,
//                    Subject: "OSR4RightsTools Account Confirm",
//                    TextBody: textBody,
//                    HtmlBody: htmlText
//                );

                var postmarkServerToken = AppConfiguration.LoadFromEnvironment().PostmarkServerToken;

                //var response = await Web.Email.Send(osrEmail, postmarkServerToken, gmailPassword);
                var response = await Web.Email.SendTemplate("register", Email, guid.ToString(), postmarkServerToken);

                if (response == false)
                {
                    // Calls to the client can throw an exception 
                    // if the request to the API times out.
                    // or if the From address is not a Sender Signature 
                    ModelState.AddModelError("Email", "Sorry problem sending the confirmation email - please try again later. We are working on resolving it.");
                    return Page();
                }

                // Notify an admin that a new user has signed up
                var notifyEmail = new OSREmail(
                    ToEmailAddress: "dave@hmsoftware.co.uk",
                    Subject: "New User Registered (arlawesi)",
                    TextBody: $"New User Registered on OSR {Email}",
                    HtmlBody: $"New User Registered on OSR {Email}"
                );

                var gmailPassword = AppConfiguration.LoadFromEnvironment().GmailPassword;
                var notifyEmailResponse = await Web.Email.Send(notifyEmail, postmarkServerToken, gmailPassword);

                return LocalRedirect("/account/register-success");
            }

            // Something failed. Redisplay the form.
            return Page();
        }
    }
}
