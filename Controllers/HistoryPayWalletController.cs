using KGQT.Business;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Controllers
{
    public class HistoryPayWalletController : Controller
    {
        #region Lịch sử giao dịch
        public IActionResult Index(int orderID, int tradeType, DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userName = userModel != null ? userModel.UserName : "";
            var oData = HistoryPayWallet.GetPage(userName, orderID, tradeType, fromDate, toDate, page, 10);
            var lstData = oData[0] as List<tbl_HistoryPayWallet>;
            var numberRecord = (int)oData[1];
            var numberPage = (int)oData[2];
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

        #region Chi tiết giao dịch
        public IActionResult Detail(int ID)
        {
            var data = HistoryPayWallet.GetByID(ID);
            ViewData["ID"] = ID;    
            return View(data);
        }
        #endregion
    }
}
