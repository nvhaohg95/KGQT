using KGQT.Business;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace KGQT.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        #region constructor
        private IToastNotification _toastNotification;
        private static ShippingOrder _shippingBus;
        private static Packages _packages;
        public HomeController(IToastNotification toastNotification)
        {
            _toastNotification = toastNotification;
            _shippingBus = new ShippingOrder();
            _packages = new Packages();
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
