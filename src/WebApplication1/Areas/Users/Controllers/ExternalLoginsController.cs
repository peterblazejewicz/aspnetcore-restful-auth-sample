using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using WebApplication1.Models;
using WebApplication1.Areas.Users.ViewModels.ExternalLogins;

namespace WebApplication1.Areas.Users.Controllers
{
    [Authorize]
    [Area("Users")]
    public class ExternalLoginsController : BaseController
    {
        public enum Messages
        {
            AddLoginSuccess,
            RemoveLoginSuccess,
            Error
        }

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public ExternalLoginsController(SignInManager<ApplicationUser> signInManager,
                                        UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        //GET: /ExternalLogins/Index
        [HttpGet]
        public async Task<IActionResult> Index(Messages? message = null)
        {
            ViewData["StatusMessage"] =
                message == Messages.RemoveLoginSuccess ? "The external login was removed."
                : message == Messages.AddLoginSuccess ? "The external login was added."
                : message == Messages.Error ? "An error has occurred."
                : string.Empty;

            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await _userManager.GetLoginsAsync(user);

            var otherLogins = _signInManager.GetExternalAuthenticationSchemes().Where(auth => userLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider)).ToList();

            ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;

            return View(new IndexViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /ExternalLogins/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action("Create", "ExternalLoginResponses");

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));

            return Challenge(properties, provider);
        }

        //
        // POST: /ExternalLogins/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeleteViewModel account)
        {
            Messages? message = Messages.Error;

            var user = await GetCurrentUserAsync();

            if (user != null)
            {
                var result = await _userManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    message = Messages.RemoveLoginSuccess;
                }
            }

            return RedirectToAction(nameof(Index), new { Message = message });
        }

        #region Helpers

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}