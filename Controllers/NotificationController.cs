using KGQT.Business;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class NotificationController : Controller
    {
        
        #region Index
        public ActionResult Index(int status , DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            var userName = HttpContext.Session.GetString("user");
            var user = AccountBusiness.GetFullInfo(null, 0, userName);
            var lstData = new List<tbl_Notification>();
            var totalRecord = 0;
            var totalPage = 0;
            ViewData["status"] = status;
            ViewData["page"] = page;
            ViewData["fromDate"] = fromDate;
            ViewData["toDate"] = toDate;
            if (user != null)
            {
                var oData = NotificationBusiness.GetPage(user.ID,status, fromDate,toDate, page, 10);
                lstData = oData[0] as List<tbl_Notification>;
                totalRecord = (int)oData[1];
                totalPage = (int)oData[2];
            }
            ViewData["totalRecord"] = totalRecord;
            ViewData["totalPage"] = totalPage;

            return View(lstData);
        }

        #endregion


        #region List notification by ID
        [HttpGet]
        public ActionResult GetList(int id, int page, int pageSize = 20)
        {
            var oData = NotificationBusiness.GetPage(id, 0, null, null, page, pageSize);
            var lstNoti = oData[0] as List<tbl_Notification>;
            return PartialView("ListNotification", lstNoti);
        }
        #endregion

    }
}
