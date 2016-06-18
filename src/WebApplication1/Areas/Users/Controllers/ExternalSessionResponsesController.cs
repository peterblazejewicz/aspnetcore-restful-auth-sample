using System.Security.Claims;
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
    /// Controller that handles a callback response from an external 
    /// session provider to authenticate the user.
    /// </summary>
    [Area("Users")]
    public class ExternalSessionResponsesController : BaseController
    {
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        public ExternalSessionResponsesController(ILoggerFactory loggerFactory,
                                                  SignInManager<ApplicationUser> signInManager)
        {
            _logger = loggerFactory.CreateLogger<SessionsController>();
            _signInManager = signInManager;
        }
        
        //
        // GET: /ExternalLoginResponses/Create
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Create(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return RedirectToAction(nameof(SessionsController.Create), "Sessions");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                return RedirectToAction(nameof(SessionsController.Create), "Sessions");
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);

                return RedirectToLocal(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(VerificationCodesController.Create), "VerificationCodes", new { ReturnUrl = returnUrl });
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;

                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                return RedirectToAction(nameof(ExternalSessionAccountsController.Create), "ExternalSessionAccounts", new CreateViewModel { Email = email });
            }
        }
    }
}
