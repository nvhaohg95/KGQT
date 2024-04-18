using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using KGQT.Business;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class ComplainController : Controller
    {
        public IActionResult Index(string transId, DateTime? fromDate, DateTime? toDate, int status, int page = 1)
        {
            var oData = ComplainBusiness.GetList(transId, fromDate, toDate,status, page);
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
            string userName = HttpContext.Session.GetString("user");
            var userLogin = AccountBusiness.GetInfo(-1, userName);
            DataReturnModel<bool> result = new();
            if(userLogin != null)
            {
                result = ComplainBusiness.UpdateStatus(ID, Status, userLogin.Username);
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
