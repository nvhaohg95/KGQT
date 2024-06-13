using KGQT.Business;
using KGQT.Commons;
using KGQT.Models.temp;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class PointsController : Controller
    {
        public IActionResult Index(string orderID, DateTime? fromDate, DateTime? toDate, int type = -1, int page = 1)
        {
            var oData = PointsBusiness.GetPage("", orderID, type, fromDate, toDate, page, 10);
            var lstData = oData[0] as List<tempPoints>;
            var numberRecord = (int)oData[1];
            var numberPage = (int)oData[2];
            ViewBag.orderID = orderID;
            ViewBag.type = type;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            return View(lstData);
        }
    }
}
