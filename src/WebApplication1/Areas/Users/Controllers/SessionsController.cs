using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using WebApplication1.Models;
using WebApplication1.Controllers;
using WebApplication1.Areas.Users.ViewModels.Sessions;

namespace WebApplication1.Areas.Users.Controllers
{
    [Area("Users")]
    public class SessionsController : BaseController
    {
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SessionsController(ILoggerFactory loggerFactory,
                                  SignInManager<ApplicationUser> signInManager)
        {
            _logger = loggerFactory.CreateLogger<SessionsController>();
            _signInManager = signInManager;
        }

        // GET: Sessions/Create
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Create(string returnUrl = null, string infoMessage = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["InfoMessage"] = infoMessage;
            return View();
        }

        // POST: Sessions/Create
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return RedirectToLocal(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(VerificationCodesController.Create), "VerificationCodes", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");

                    return View("LockedOut");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        // POST: Sessions/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete()
        {
            await _signInManager.SignOutAsync();

            _logger.LogInformation(4, "User logged out.");

            return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "default" });
        }
    }
}