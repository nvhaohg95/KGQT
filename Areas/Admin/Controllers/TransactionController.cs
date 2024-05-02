using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class TransactionController : Controller
    {
        #region View
        public IActionResult Index()
        {
            return View();
        }

        #endregion

        #region Recharge
        //[HttpGet]
        //public IActionResult Recharge(int status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        //{
        //    var oData = TransactionBusiness.GetListTransaction(status, fromDate, toDate, page, pageSize);
        //    var lstData = oData[0] as List<tbl_Transaction>;
        //    var totalRecord = (int)oData[1];
        //    var totalPage = (int)oData[2];
        //    ViewData["status"] = status;
        //    ViewData["fromDate"] = fromDate;
        //    ViewData["toDate"] = toDate;
        //    ViewData["page"] = page;
        //    ViewData["totalRecord"] = totalRecord;
        //    ViewData["totalPage"] = totalPage;
        //    return View(lstData);
        //}


        [HttpPost]
        public object Recharge(int ID)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var result = TransactionBusiness.ApprovalRecharge(ID, userLogin);
            return result;
        }

        #endregion


        
    }
}
