using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using WebApplication1.Models;

namespace WebApplication1.Areas.Users.Controllers
{
    /// <summary>
    /// Controller that handles a callback response from an external 
    /// login provider when the user links one to their account.
    /// </summary>
    [Authorize]
    [Area("Users")]
    public class ExternalLoginResponsesController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public ExternalLoginResponsesController(SignInManager<ApplicationUser> signInManager,
                                                UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        //
        // GET: /ExternalLoginResponses/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));

            if (info == null)
            {
                return RedirectToAction(
                    nameof(ExternalLoginsController.Index), 
                    "ExternalLogins", 
                    new { Message = ExternalLoginsController.Messages.Error });
            }

            var result = await _userManager.AddLoginAsync(user, info);

            var message = result.Succeeded ? 
                ExternalLoginsController.Messages.AddLoginSuccess : 
                ExternalLoginsController.Messages.Error;

            return RedirectToAction(
                nameof(ExternalLoginsController.Index), 
                "ExternalLogins", 
                new { Message = message });
        }

        #region Helpers

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}
