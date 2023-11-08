using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing.Printing;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class WithDrawController : Controller
    {
        #region View
        public IActionResult Index(int status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            var oData = WithDraw.GetList(status, fromDate, toDate, page, pageSize);
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
            var user = Accounts.GetFullInfo(null, -1, model.Username);
            if (user != null)
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
                var u = BusinessBase.GetOne<tbl_Account>(x=>x.ID== user.ID);
                u.Wallet += model.Amount;
                s = BusinessBase.Update(u);

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
    }
}
