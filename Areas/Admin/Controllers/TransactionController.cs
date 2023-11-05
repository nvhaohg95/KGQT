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

        public IActionResult Create()
        {
            return View();
        }
        #endregion

        #region Function
        [HttpPost]
        public IActionResult Insert(tbl_Withdraw model)
        {
            var user = Accounts.GetFullInfo(null, -1, model.Username);
            if(user != null)
            {
                model.UID = user.ID; 
                model.Fullname = user.FirstName + " " + user.LastName;
            }
            model.Status = 2;
            model.Type = 1;
            model.CreatedBy = HttpContext.Session.GetString("user");
            model.CreatedDate = DateTime.Now;
            var s = BusinessBase.Add(model);
            if (s)
            {
                user.Wallet += model.Amount;
              s = BusinessBase.Update(model);

            }
            return Ok(s);
        }

        [HttpGet]
        public IActionResult AutoComplete(string s)
        {
            var data = BusinessBase.GetList<tbl_Account>(x => x.Username.Contains(s)).ToList();
            return Json(data);
        }

        #endregion

        #region Recharge
        [HttpGet]
        public IActionResult Recharge(int status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            var oData = TransactionBusiness.GetListTransaction(status, fromDate, toDate, page, pageSize);
            var lstData = oData[0] as List<tbl_Transaction>;
            var totalRecord = (int)oData[1];
            var totalPage = (int)oData[2];
            ViewData["status"] = status;
            ViewData["fromDate"] = fromDate;
            ViewData["toDate"] = toDate;
            ViewData["page"] = page;
            ViewData["totalRecord"] = totalRecord;
            ViewData["totalPage"] = totalPage;
            return View(lstData);
        }


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
