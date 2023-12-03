using KGQT.Business;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class HistoryPayWalletController : Controller
    {
        #region Lịch sử giao dịch
        public IActionResult Index(int orderID, int tradeType, DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            var userName = HttpContext.Session.GetString("user");
            var oData = HistoryPayWallet.GetPage(userName, orderID, tradeType, fromDate, toDate, page, 10);
            var lstData = oData[0] as List<tbl_HistoryPayWallet>;
            var totalRecord = (int)oData[1];
            var totalPage = (int)oData[2];
            ViewData["orderID"] = orderID;
            ViewData["tradeType"] = tradeType;
            ViewData["fromDate"] = fromDate;
            ViewData["toDate"] = toDate;
            ViewData["page"] = page;
            ViewData["totalRecord"] = totalRecord;
            ViewData["totalPage"] = totalPage;
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
