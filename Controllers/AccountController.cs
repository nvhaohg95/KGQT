using KGQT.Business;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Controllers
{
    public class AccountController : Controller
    {
        #region Info
        public IActionResult Info()
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var accInfo = AccountBusiness.GetInfo(-1, userLogin);
            return View(accInfo);
        }

        #endregion

        #region Update Info
        [HttpPost]
        public object Update(SignUpModel data)
        {
            if (data == null)
            {
                return new DataReturnModel<object>() { IsError = true, Message = "Cập nhật không thành công. Vui lòng thử lại!" };
            }
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            if(!string.IsNullOrEmpty(userLogin))
            {
                return AccountBusiness.Update(data, data.File, userLogin);
            }
            else 
            { 
                return new DataReturnModel<object>() { IsError = true, Message = "Cập nhật không thành công. Vui lòng thử lại!" };
            }
        }

        #endregion
    }
}
