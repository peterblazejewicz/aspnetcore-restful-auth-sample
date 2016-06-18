using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using WebApplication1.Models;

namespace WebApplication1.Areas.Users.Controllers
{
    [Area("Users")]
    public class ExternalSessionsController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        public ExternalSessionsController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        // POST: /ExternalSessions/Create
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string provider, string returnUrl = null)
        {
            // Request a redirect to the external session provider.
            var redirectUrl = Url.Action("Create", "ExternalSessionResponses", new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }
    }
}