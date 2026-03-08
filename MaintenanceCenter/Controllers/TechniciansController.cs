using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceCenter.Web.Controllers
{

    [Authorize(Roles = "Technician")] 
    public class TechniciansController : Controller
    {
        public IActionResult Workspace()
        {
            return View();
        }

        //// Monitoring
        //public IActionResult Monitoring() => View();

    }
}
