using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using KGQT.Business;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KGQT.Controllers
{
    public class NotificationController : Controller
    {
        
        #region Index
        public ActionResult Index(int status , DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            var userName = HttpContext.Request.Cookies["user"];
            var user = AccountBusiness.GetInfo(-1, userName);
            var lstData = new List<tbl_Notification>();
            int numberRecord = 0;
            int numberPage = 0;
            if (user != null)
            {
                var oData = NotificationBusiness.GetPage(user.ID,status, fromDate,toDate, page, 10);
                lstData = oData[0] as List<tbl_Notification>;
                numberRecord = (int)oData[1];
                numberPage = (int)oData[2];
            }
            ViewBag.status = status;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;

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

        #region Get detail
        public ActionResult Detail(int id)
        {
            var userName = HttpContext.Request.Cookies["user"];
            var user = AccountBusiness.GetInfo(-1, userName);
            var lstData = new List<tbl_Notification>();
            var numberRecord = 0;
            var numberPage = 0;
            ViewBag.status = 0;
            ViewBag.fromDate = null;
            ViewBag.toDate = null;
            if (user != null)
            {
                var oData = NotificationBusiness.GetDetail(id, userName);
                lstData = oData[0] as List<tbl_Notification>;
                numberRecord = (int)oData[1];
                numberPage = (int)oData[2];
            }
            ViewBag.pageCurrent = 1;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;

            return View("Index", lstData);
        }
        #endregion

        #region Update status
        [HttpPost]
        public bool UpdateStatus(int ID)
        {
            var userName = HttpContext.Request.Cookies["user"];
            NotificationBusiness.UpdateStatus(ID, userName);
            return true;
        }
        #endregion
    }
}
