using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using WebApplication1.Controllers;

namespace WebApplication1
{
    /// <summary>
    /// Base class from which all Controllers should inherit. 
    /// Add protected methods here to make them available to 
    /// all of your Controllers.
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "default" });
            }
        }

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
