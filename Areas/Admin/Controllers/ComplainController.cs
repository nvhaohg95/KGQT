using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using KGQT.Business;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class ComplainController : Controller
    {
        public IActionResult Index(int? ID,string transId, DateTime? fromDate, DateTime? toDate, int status, int page = 1)
        {
            var oData = ComplainBusiness.GetList(ID,transId, fromDate, toDate,status, page);
            var lstData = oData[0] as List<tbl_Complain>;
            var numberRecord = (int)oData[1];
            var numberPage = (int)oData[2];
            ViewBag.transId = transId;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.status = status;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            return View(lstData);
        }

        public IActionResult Detail(int ID) 
        {
            var data = ComplainBusiness.GetByID(ID);
            return View(data);
        }

        public DataReturnModel<bool> UpdateStatus(int ID,int Status)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var user = AccountBusiness.GetInfo(-1, userLogin);
            DataReturnModel<bool> result = new();
            if(user != null)
            {
                result = ComplainBusiness.UpdateStatus(ID, Status, userLogin);
            }
            else
            {
                result.IsError = true;
                result.Data = false;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
            }
            return result;
        }
    }
}
