using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using WebApplication1.Models;
using WebApplication1.Controllers;
using WebApplication1.Areas.Users.ViewModels.Passwords;

namespace WebApplication1.Areas.Users.Controllers
{
    [Area("Users")]
    public class PasswordsController : BaseController
    {
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public PasswordsController(ILoggerFactory loggerFactory,
                                   UserManager<ApplicationUser> userManager)
        {
            _logger = loggerFactory.CreateLogger<PasswordsController>();
            _userManager = userManager;
        }

        // GET: Passwords/Create
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Passwords/Create
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);

                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(HomeController.Index), "Home", new { infoMessage = "We received your password reset request. Please check your email to reset your password.", area = "default" });
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var callbackUrl = Url.Action("Edit", "Passwords", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                //   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                //return RedirectToAction(nameof(HomeController.Index), "Home", new { infoMessage = "We received your password reset request. Please check your email to reset your password." });
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: Passwords/Edit
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Edit(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        // POST: Passwords/Edit
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(SessionsController.Create), "Sessions", new { infoMessage = "Your password has been reset. Please login below." });
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(SessionsController.Create), "Sessions", new { infoMessage = "Your password has been reset. Please login below." });
            }

            AddErrors(result);

            return View();
        }
    }
}