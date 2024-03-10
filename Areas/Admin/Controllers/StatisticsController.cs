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
            toDate = toDate.Value.AddDays(1).AddTicks(-1);

            var total =StatisticsBusiness.Revenue(fromDate, toDate, page,pageSize);
            return View(total);
        }
    }
}
