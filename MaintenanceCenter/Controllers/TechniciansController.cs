using Microsoft.AspNetCore.Mvc;

namespace MaintenanceCenter.Web.Controllers
{
    public class TechniciansController : Controller
    {
        public IActionResult Workspace()
        {
            return View();
        }
    }
}
