using KGQT.Base;
using KGQT.Business;
using KGQT.Controllers;
using Microsoft.AspNetCore.Mvc;


namespace KGQT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        #region constructor

        public HomeController()
        {

        }
        #endregion

        #region Index/Home
        public IActionResult Index()
        {
            var pack = PackagesBusiness.GetAllStatus("");
            ViewData["pack_st3"] = pack[0];
            ViewData["pack_st4"] = pack[1];
            ViewData["pack_st5"] = pack[2];

            var ship = ShippingOrder.GetAllStatus("");
            ViewData["order_st1"] = ship[0];
            ViewData["order_st2"] = ship[1];
            ViewData["ordertotal"] = ship[2];
            return View();
        }

        #endregion

        #region Dasboard
        public IActionResult Dashboard()
        {
            return View();
        }
        #endregion

    }
}
