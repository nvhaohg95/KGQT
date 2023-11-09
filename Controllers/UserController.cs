using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace KGQT.Controllers
{
    public class UserController : Controller
    {
        #region constructor
        private IToastNotification _toastNotification;
        public IToastNotification NotiService { 
            get { return _toastNotification; }
            set { _toastNotification = value; } 
        }

        public UserController(IToastNotification toastNotification)
        {
            NotiService = toastNotification;
        }
        #endregion

        public ActionResult Detail(int id)
        {
            var user = UserBusiness.GetUser(id);
            ViewData["ID"] = id;
            return View(user);
        }

        #region Update
        [HttpPost]
        public object Update(int ID, AccountInfo form)
        {
            var user = BusinessBase.GetOne<tbl_Account>(x => x.ID == ID);
            if (user == null) return new { error = true, mssg = "Không tìm thấy thông tin tài khoản" };
            var accInfo = BusinessBase.GetOne<tbl_AccountInfo>(x => x.UID == ID);
            if(accInfo == null) return new { error = true, mssg = "Không tìm thấy thông tin tài khoản" };
            accInfo.FirstName = form.FirstName;
            accInfo.LastName = form.LastName;
            accInfo.Gender = form.Gender;
            accInfo.BirthDay = form.BirthDay;
            accInfo.Email = form.Email;
            accInfo.Phone = form.Phone;
            accInfo.Address = form.Address;
            accInfo.ModifiedBy = HttpContext.Session.GetString("user");
            accInfo.ModifiedDate = DateTime.Now;
            user.ModifiedBy = HttpContext.Session.GetString("user");
            user.ModifiedDate = DateTime.Now;
            var isSave = BusinessBase.Update(accInfo);
            BusinessBase.Update(user);
            if (isSave)
            {
                return new { error = false };
            }
            return new { error = true, mssg = "Không tìm thấy thông tin" };
        }
        #endregion

    }
}
