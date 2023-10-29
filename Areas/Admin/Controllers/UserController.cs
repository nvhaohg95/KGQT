using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models;
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

        #region Update
        public object Update(tbl_Account form)
        {
            var user = BusinessBase.GetOne<tbl_Account>(x => x.ID == form.ID);
            if (user == null) return new { error = true, mssg = "Không tìm thấy thông tin" };
            if (user.UserID == form.UserID) return new { error = false };
            var exist = BusinessBase.Exist<tbl_Account>(x => x.UserID == form.UserID && x.ID != user.ID);
            if (exist)
                return new { error = true, mssg = "Mã định danh đã tồn tại" };

            user.UserID = form.UserID;
            user.RoleID = form.RoleID;
            user.ModifiedBy = HttpContext.Session.GetString("user");
            user.ModifiedDate = DateTime.Now;
            var s = BusinessBase.Update(user);
            if (s)
                return new { error = false };
            return new { error = true, mssg = "Không tìm thấy thông tin" };
        }
        #endregion
    }
}
