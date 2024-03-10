using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class StatisticsController : Controller
    {

        /// <summary>
        /// Thống kê doanh thu
        /// </summary>
        /// <returns></returns>
        public IActionResult StatisticRevenue(DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20)
        {
            if(fromDate == null)
                fromDate = DateTime.Now;
            
            if(toDate == null)
                toDate = DateTime.Now;

            fromDate = fromDate.Value.Date;
            toDate = toDate.Value.Date.AddDays(1).AddTicks(-1);

            var oData = StatisticsBusiness.Revenue(fromDate, toDate, page,pageSize);

            var lstData = oData[0] as List<tbl_ShippingOrder>;
            int numberRecord = (int)oData[1];
            int numberPage = (int)oData[2];
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            return View(lstData);
        }
    }
}
