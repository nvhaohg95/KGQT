using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class UserController : Controller
    {
        #region Index
        public IActionResult Index(int page, string searchText = "")
        {
            var oData = UserBusiness.GetListUser(searchText, page);
            var lst = oData[0] as List<AccountInfo>;
            decimal total = (decimal)oData[1];
            decimal totalPage = (decimal)oData[2];
            ViewData["page"] = page != 0 ? page : 1;
            ViewData["total"] = total;
            ViewData["totalPage"] = totalPage;
            var userVM = new UserVM();
            userVM.ListUser = lst;
            userVM.User = new AccountInfo();
            return View(userVM);
        }

        #endregion

        #region Detail
        [HttpGet]
        public IActionResult Detail(int id)
        {
            var user = UserBusiness.GetUser(id);
            return View(user);
        }
        #endregion

        #region Create
        [HttpPost]
        public JsonResult Create(AccountInfo data)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var reponse = UserBusiness.AddUser(data, userLogin); 
            return Json(reponse);
        }
        #endregion

        #region Delete
        [HttpPost]
        public bool Delete(int id)
        {
            var isDelete = UserBusiness.DeleteUser(id);
            return isDelete;
        }
        #endregion
    }
}
