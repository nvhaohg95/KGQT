using KGQT.Business;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        #region constructor
        private IToastNotification _toastNotification;
        public HomeController(IToastNotification toastNotification)
        {
            _toastNotification = toastNotification;
        }
        #endregion

        #region Index/Home
        public IActionResult Index()
        {
            return View();
        }

        #endregion

    }
}
