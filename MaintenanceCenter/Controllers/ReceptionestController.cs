using Microsoft.AspNetCore.Mvc;

namespace MaintenanceCenter.Web.Controllers.Mvc
{
    public class ReceptionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}