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
        [HttpGet]
        public IActionResult Create()
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var user = AccountBusiness.GetInfo(-1, userModel.UserName);
            ViewBag.userName = user.Username;
            ViewBag.wallet = Converted.ToDouble(user.Wallet);
            return View();
        }
        [HttpPost]
        public DataReturnModel<bool> Create(string username,int point)
        {
            DataReturnModel<bool> result = new();
            if(!string.IsNullOrEmpty(username) && point > 0)
            {
                result = WithDrawBusiness.BuySearches(username, point, false);
            }
            else
            {
                result.IsError = true;
                result.Message = "Đổi điểm không thành công. Vui lòng thử lại!";
            }
            return result;
        }
    }
}
