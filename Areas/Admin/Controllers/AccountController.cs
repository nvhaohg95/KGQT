using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]

    public class AccountController : Controller
    {

        #region Index
        [HttpGet]
        public IActionResult Index(string searchText, int page = 1, int pageSize = 10)
        {
            var oData = AccountBusiness.GetPage(searchText, page, pageSize);
            var lstData = oData[0] as List<tbl_Account>;
            int totalRecord = (int)oData[1];
            int totalPage = (int)oData[2];
            @ViewData["searchText"] = searchText;
            @ViewData["page"] = page;
            @ViewData["totalRecord"] = totalRecord;
            @ViewData["totalPage"] = totalPage;
            @ViewData["lstRoles"] = AccountBusiness.GetListUserRole();
            return View(lstData);
        }
        #endregion

        #region Detail
        [HttpGet]
        public IActionResult Detail(int id)
        {
            var user = UserBusiness.GetUserByID(id);
            ViewData["ID"] = id;
            ViewData["lstRoles"] = AccountBusiness.GetListUserRole();
            return View(user);
        }

        #endregion

        #region Create
        [HttpPost]
        public JsonResult Create(AccountInfo data)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var reponse = AccountBusiness.Create(data, userLogin);
            return Json(reponse);
        }

        #endregion

        #region Update
        [HttpPost]
        public object Update(AccountInfo data)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var reponse = AccountBusiness.Update(data, userLogin);
            return reponse;
        }

        #endregion

        #region Delete
        [HttpPost]
        public bool Delete(int id)
        {
            var isDelete = AccountBusiness.Delete(id);
            return isDelete;
        }

        #endregion

        #region Get Infor
        [HttpGet]
        public IActionResult Info()
        {
            var userName = HttpContext.Session.GetString("user");
            var accInfo = AccountBusiness.GetInfo(0,userName);
            ViewData["userName"] = userName;
            ViewData["lstRoles"] = AccountBusiness.GetListUserRole();
            return View(accInfo);
        }

        #endregion

        #region Update Info
        [HttpPost]
        public object UpdateInfo(tbl_Account data)
        {
            var reponse = AccountBusiness.UpdateInfo(data);
            return reponse;
        }

        #endregion
    }
}
