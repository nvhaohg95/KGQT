using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using KGQT.Business;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace KGQT.Areas.Controllers
{
    [Area("admin")]
    public class NotificationController : Controller
    {
        #region Index
        public ActionResult Index(int status , DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            var oData = NotificationBusiness.GetPage(0, status, fromDate, toDate, page, 10, true);
            var lstData = oData[0] as List<tbl_Notification>;
            var totalRecord = (int)oData[1];
            var totalPage = (int)oData[2];
            ViewData["status"] = status;
            ViewData["page"] = page;
            ViewData["fromDate"] = fromDate;
            ViewData["toDate"] = toDate;
            ViewData["totalRecord"] = totalRecord;
            ViewData["totalPage"] = totalPage;
            return View(lstData);
        }

        #endregion

        #region List notification
        [HttpGet]
        public ActionResult GetList(int page, int pageSize = 10)
        {
            var oData = NotificationBusiness.GetPage(0, 0,null, null, page, pageSize,true);
            var lstNoti = oData[0] as List<tbl_Notification>;
            return PartialView("ListNotification", lstNoti);
        }
        #endregion

        #region Get detail
        public ActionResult Detail(int id)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            ViewData["status"] = 0;
            ViewData["page"] = 1;
            ViewData["fromDate"] = null;
            ViewData["toDate"] = null;
            var oData = NotificationBusiness.GetDetail(id, userLogin);
            var lstData = oData[0] as List<tbl_Notification>;
            var totalRecord = (int)oData[1];
            var totalPage = (int)oData[2];
            ViewData["totalRecord"] = totalRecord;
            ViewData["totalPage"] = totalPage;

            return View("Index", lstData);
        }
        #endregion

        #region Update status
        [HttpPost]
        public bool UpdateStatus(int ID)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var result = NotificationBusiness.UpdateStatus(ID, userLogin);
            return result;
        }
        #endregion

        #region Create
        public IActionResult Create()
        {
            return View();
        }
        #endregion

        #region Add
        [HttpPost]
        public DataReturnModel<bool> Create(NotificationViewModel model)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            DataReturnModel<bool> result = new();
            if(!string.IsNullOrEmpty(userLogin))
            {
                result = NotificationBusiness.SendNotiForUser(model.To, model.Contents, model.SendToAll, userLogin);
            }
            else
            {
                result.IsError = true;
                result.Data = false;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
            }
            return result;

        }
        
        #endregion
    }
}
