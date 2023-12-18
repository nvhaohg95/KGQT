using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Drawing.Printing;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class WithDrawController : Controller
    {
        #region View
        public IActionResult Index(int status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            var oData = WithDrawBusiness.GetList(status, fromDate, toDate, page, pageSize);
            var lstData = oData[0] as List<tbl_Withdraw>;
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
        [HttpGet]
        public IActionResult Create(string user)
        {
            ViewData["user"] = user;
            return View();
        }
        #endregion

        #region Function
        [HttpPost]
        public IActionResult Insert(tbl_Withdraw model)
        {
            var user = AccountBusiness.GetInfo(-1, model.Username);
            var userName = HttpContext.Session.GetString("user");
            if (user != null)
            {
                model.UID = user.ID;
                model.Fullname = user.FullName;
            }
            model.Status = 2;
            model.Type = 1;
            model.CreatedBy = userName;
            model.CreatedDate = DateTime.Now;
            var s = BusinessBase.Add(model);
            if (s)
            {
                var u = BusinessBase.GetOne<tbl_Account>(x => x.ID == user.ID);
                u.Wallet += model.Amount;
                s = BusinessBase.Update(u);

                #region Logs
                HistoryPayWallet.Insert(u.ID, u.Username, model.ID, model.Note, model.Amount.Value, 1, 1, u.Wallet.Value, userName);
                #endregion
            }
            return Ok(s);
        }



        [HttpPost]
        public object Arppoval(int id)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var result = TransactionBusiness.ApprovalRecharge(id, userLogin);
            return result;
        }
        [HttpGet]
        public IActionResult AutoComplete(string s)
        {
            var data = BusinessBase.GetList<tbl_Account>(x => x.Username.Contains(s)).ToList();
            return Json(data);
        }
        #endregion

        #region Tạo lệnh nạp tiền
        [HttpPost]
        public bool Create(tbl_Withdraw model)
        {
            string userLogin = HttpContext.Session.GetString("user");
            var user = AccountBusiness.GetInfo(-1, model.Username);
            if (user != null && !string.IsNullOrEmpty(userLogin))
            {
                model.UID = user.ID;
                model.Fullname = user.FullName;
                model.Status = 2;
                model.Type = 1;
                model.CreatedBy = userLogin;
                model.CreatedDate = DateTime.Now;
                var result = WithDrawBusiness.Insert(model, userLogin);
                if (result)
                {
                    var admin = AccountBusiness.GetInfo(-1, userLogin);
                    if (model.UID == admin.ID)
                        HttpContext.Session.SetString("US_LOGIN", JsonConvert.SerializeObject(admin));
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Duyệt lệnh nạp tiền
        public bool Approval(int ID)
        {
            string userLogin = HttpContext.Session.GetString("user");
            var result = WithDrawBusiness.Approval(ID, userLogin);
            if (result != null)
            {
                var user = AccountBusiness.GetInfo(-1, userLogin);
                var data = result.Data as tbl_Withdraw;
                if (data.UID == user.ID)
                    HttpContext.Session.SetString("US_LOGIN", JsonConvert.SerializeObject(user));
                return true;
            }
            return false;
        }
        #endregion

    }
}
