using KGQT.Business;
using KGQT.Commons;
using KGQT.Models.temp;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class HistoryPayWalletController : Controller
    {
        public IActionResult Index(string username, int tradeType, DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            var oData = HistoryPayWallet.GetPage(username, 0, tradeType, fromDate, toDate, page, 10);
            var lstData = oData[0] as List<tbl_HistoryPayWallet>;
            var numberRecord = (int)oData[1];
            var numberPage = (int)oData[2];
            ViewBag.Username = username;
            ViewBag.tradeType = tradeType;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            return View(lstData);
        }

    }
}
