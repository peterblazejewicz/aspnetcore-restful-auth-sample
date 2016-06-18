using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using WebApplication1.Models;

namespace WebApplication1.Areas.Users.Controllers
{
    /// <summary>
    /// Controller that handles a user clicking a link in an email 
    /// to confirm their account.
    /// </summary>
    [Area("Users")]
    public class EmailConfirmationsController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        /// <summary>
        /// Creates a new <see cref="EmailConfirmationsController"/>.
        /// </summary>
        /// <param name="userManager">The ASP.NET Identity UserManager to use.</param>
        public EmailConfirmationsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        // GET: /EmailConfirmations/Create
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Create(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            return View(result.Succeeded ? "Create" : "Error");
        }
    }
}
