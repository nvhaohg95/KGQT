using Microsoft.AspNetCore.Mvc;

namespace KGQT.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        #region Index/Home
        public IActionResult Index()
        {
            return View();
        }

        #endregion

    }
}
