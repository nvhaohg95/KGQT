using Microsoft.AspNetCore.Mvc;

namespace KGQT.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
