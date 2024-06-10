using KGQT.Business;
using KGQT.Commons;
using KGQT.Models.temp;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Controllers
{
    public class PointsController : Controller
    {
        public IActionResult Index(string orderID, DateTime? fromDate, DateTime? toDate, int type = -1, int page = 1)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userName = userModel != null ? userModel.UserName : "";
            var oData = PointsBusiness.GetPage(userName, orderID, type, fromDate, toDate, page, 10);
            var lstData = oData[0] as List<tbl_Point>;
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
