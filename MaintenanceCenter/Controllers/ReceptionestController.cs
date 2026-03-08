using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceCenter.Web.Controllers.Mvc
{
    [Authorize(Roles ="Receptionist")]
    public class ReceptionController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }
    }
}