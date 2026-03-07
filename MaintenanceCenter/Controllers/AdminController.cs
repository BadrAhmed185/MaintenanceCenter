using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceCenter.Web.Controllers.Mvc
{
    [Route("Admin")]
    [Authorize]
    public class AdminController : Controller
    {
        [HttpGet("Dashboard")]
        public IActionResult Dashboard() => View();

        [HttpGet("Users")]
        public IActionResult Users() => View();

        [HttpGet("Workshops")]
        public IActionResult Workshops() => View();

        [HttpGet("SpareParts")]
        public IActionResult SpareParts() => View();

        [HttpGet("MaintenanceServices")]
        public IActionResult MaintenanceServices() => View();


        [HttpGet("Monitoring")]
        public IActionResult Monitoring() => View();
    }
}



// Ok but we have a lot of things broken , first the spare parts page is not displaying or saving anything because it is just cshtml file and contains not logic handler because you did not gave me a js code that handles the logic and calls , second thing that i need you to give me the /Admin/MaintenanceServices, and /Admin/Workshops pages and js code. because writing them will consume time .

// finally these separate pages now will be stand alone pages i reach them through the url and of course this is not a ux because we are building a prototype for the users not for the developers