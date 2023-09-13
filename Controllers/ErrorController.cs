using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
