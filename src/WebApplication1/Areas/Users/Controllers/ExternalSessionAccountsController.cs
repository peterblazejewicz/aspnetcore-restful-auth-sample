using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using WebApplication1.Models;
using WebApplication1.Areas.Users.ViewModels.ExternalSessionAccounts;

namespace WebApplication1.Areas.Users.Controllers
{
    /// <summary>
    /// Controller that handles creating an account for a 
    /// new user that used an external session provider 
    /// to authenticate for the first time.
    /// </summary>
    [Area("Users")]
    public class ExternalSessionAccountsController : BaseController
    {
        private readonly ILogger _logger;

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExternalSessionAccountsController(ILoggerFactory loggerFactory,
                                                 SignInManager<ApplicationUser> signInManager,
                                                 UserManager<ApplicationUser> userManager)
        {
            _logger = loggerFactory.CreateLogger<ExternalSessionAccountsController>();
            _signInManager = signInManager;
            _userManager = userManager;
        }

        //
        // POST: /ExternalSessionAccounts/Create
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();

                if (info == null)
                {
                    return RedirectToAction(nameof(SessionsController.Create), "Sessions", new { infoMessage = "Unsuccessful login with service." });
                }

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);

                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }
    }
}