using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class UserController : Controller
    {
        #region Index
        public IActionResult Index(string searchText ,int page = 1 ,int pageSize = 10)
        {
            var oData = UserBusiness.GetPage(searchText, page, pageSize);
            var lstData = oData[0] as List<AccountInfo>;
            int totalRecord = (int)oData[1];
            int totalPage = (int)oData[2];
            @ViewData["searchText"] = searchText;
            @ViewData["page"] = page;
            @ViewData["totalRecord"] = totalRecord;
            @ViewData["totalPage"] = totalPage;
            @ViewData["lstRoles"] = GetListUserRole();
            return View(lstData);
        }

        #endregion

        #region Detail
        [HttpGet]
        public IActionResult Detail(int id)
        {
            var user = UserBusiness.GetUser(id);
            ViewData["lstRoles"] = GetListUserRole();
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


        #region Get User Infor
        public IActionResult Infor()
        {
            var userLogin = HttpContext.Session.GetString("user");
            var user = UserBusiness.GetUserInfor(userLogin);
            @ViewData["lstRoles"] = GetListUserRole();
            return View(user);
        }
        #endregion

        private List<UserRole> GetListUserRole()
        {
           List<UserRole> lst = new List<UserRole>();
            using (var db = new nhanshiphangContext())
            {
                lst = db.tbl_Roles.OrderBy(x => x.RoleID).Select(x => new UserRole()
                {
                    RoleID = x.RoleID,
                    RoleName = x.RoleName
                }).ToList();
            }
            return lst;
        }
    }

    
}
