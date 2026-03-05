using Microsoft.AspNetCore.Mvc;

namespace MaintenanceCenter.Web.Controllers.Mvc
{
    // Route it nicely to /Auth/Login
    [Route("Auth")]
    public class AuthController : Controller
    {
        [HttpGet("Login")]
        public IActionResult Login()
        {
            // If the user is already authenticated, redirect them to the app
            //if (User.Identity != null && User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("Index", "Reception"); // Or route based on role later
            //}

            return View();
        }
    }
}