using KGQT.Business;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Areas.Controllers
{
    [Area("admin")]
    public class NotificationController : Controller
    {
        
        #region Index
        public ActionResult Index(int status , DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            ViewData["status"] = status;
            ViewData["page"] = page;
            ViewData["fromDate"] = fromDate;
            ViewData["toDate"] = toDate;
            var oData = NotificationBusiness.GetPage(0, status, fromDate, toDate, page, 10, true);
            var lstData = oData[0] as List<tbl_Notification>;
            var totalRecord = (int)oData[1];
            var totalPage = (int)oData[2];
            ViewData["totalRecord"] = totalRecord;
            ViewData["totalPage"] = totalPage;

            return View(lstData);
        }

        #endregion


        #region List notification by ID
        [HttpGet]
        public ActionResult GetList(int id, int page, int pageSize = 20)
        {
            var oData = NotificationBusiness.GetPage(0, 0, null, null, page, pageSize,true);
            var lstNoti = oData[0] as List<tbl_Notification>;
            return PartialView("ListNotification", lstNoti);
        }
        #endregion

    }
}
