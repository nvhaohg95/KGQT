using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class TransactionController : Controller
    {
        #region View
        public IActionResult Index(string kh, int orderID, int tradeType, DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            var oData = HistoryPayWallet.GetPage(kh, orderID, tradeType, fromDate, toDate, page, 10);
            var lstData = oData[0] as List<tbl_HistoryPayWallet>;
            int numberRecord = (int)oData[1];
            int numberPage = (int)oData[2];
            ViewBag.kh = kh;
            ViewBag.orderID = orderID;
            ViewBag.tradeType = tradeType;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            return View(lstData);
        }

        #endregion

        #region Recharge
        //[HttpGet]
        //public IActionResult Recharge(int status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        //{
        //    var oData = TransactionBusiness.GetListTransaction(status, fromDate, toDate, page, pageSize);
        //    var lstData = oData[0] as List<tbl_Transaction>;
        //    var totalRecord = (int)oData[1];
        //    var totalPage = (int)oData[2];
        //    ViewData["status"] = status;
        //    ViewData["fromDate"] = fromDate;
        //    ViewData["toDate"] = toDate;
        //    ViewData["page"] = page;
        //    ViewData["totalRecord"] = totalRecord;
        //    ViewData["totalPage"] = totalPage;
        //    return View(lstData);
        //}


        [HttpPost]
        public object Recharge(int ID)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var result = TransactionBusiness.ApprovalRecharge(ID, userLogin);
            return result;
        }

        #endregion
        
    }
}
