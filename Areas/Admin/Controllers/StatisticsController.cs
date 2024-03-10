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
        /// Thống kê cân nặng
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult StatisticWeight(DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20)
        {
            if (fromDate == null)
                fromDate = DateTime.Now;

            if (toDate == null)
                toDate = DateTime.Now;

            fromDate = fromDate.Value.Date;
            toDate = toDate.Value.Date.AddDays(1).AddTicks(-1);

            var oData = StatisticsBusiness.PackageWeight(fromDate, toDate, page, pageSize);

            var lstData = oData[0] as List<tbl_Package>;
            int numberRecord = (int)oData[1];
            int numberPage = (int)oData[2];
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            return View(lstData);
        }
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

        /// <summary>
        /// Thống kê số tiền khách phải trả
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult StatisticCustomerDebt(DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20)
        {
            if (fromDate == null)
                fromDate = DateTime.Now;

            if (toDate == null)
                toDate = DateTime.Now;

            fromDate = fromDate.Value.Date;
            toDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            var oData = StatisticsBusiness.CustomerDebt(fromDate, toDate);
            ViewBag.Data = oData;
            return View();
        }


    }
}
