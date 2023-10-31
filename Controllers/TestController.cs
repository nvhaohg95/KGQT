using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
