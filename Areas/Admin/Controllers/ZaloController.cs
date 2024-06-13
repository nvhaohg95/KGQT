using KGQT.Business;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Areas.Admin.Controllers
{
    public class ZaloController : Controller
    {
        [Area("admin")]
        public IActionResult Index(string search, int page = 1)
        {
            var oData = ZaloBusiness.GetPage(search, page, 10);
            var lstData = oData[0] as List<tbl_ZaloFollewer>;
            ViewBag.numberPage = (int)oData[2];
            ViewBag.numberRecord = (int)oData[1];
            ViewBag.pageCurrent = page;
            return View(lstData);
        }
    }
}
