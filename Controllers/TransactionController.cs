using DocumentFormat.OpenXml.Office2010.Excel;
using KGQT.Business;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class TransactionController : Controller
    {
        public IActionResult Index(int orderID, int tradeType, DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            var userName = HttpContext.Session.GetString("user");
            var oData = HistoryPayWallet.GetPage(userName, orderID, tradeType, fromDate, toDate, page, 10);
            var lstData = oData[0] as List<tbl_HistoryPayWallet>;
            int numberRecord = (int)oData[1];
            int numberPage = (int)oData[2];
            ViewBag.orderID = orderID;
            ViewBag.tradeType = tradeType;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            return View(lstData);
        }

        public IActionResult Detail(int ID)
        {
            ViewData["ID"] = ID;
            return View();
        }

        #region Nạp tiền
        public IActionResult Recharge()
        {
            return View();
        }
        #endregion

        #region Transaction Log
        public IActionResult History(int orderID,int tradeType,DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            var userName = HttpContext.Session.GetString("user");
            var oData = HistoryPayWallet.GetPage(userName, orderID, tradeType, fromDate, toDate, page, 10);
            var lstData = oData[0] as List<tbl_HistoryPayWallet>;
            int numberRecord = (int)oData[1];
            int numberPage = (int)oData[2];
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
    }
}
