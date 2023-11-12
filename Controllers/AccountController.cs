using KGQT.Business;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class AccountController : Controller
    {
        #region Info
        public IActionResult Info()
        {
            var userName = HttpContext.Session.GetString("user");
            var accInfo = AccountBusiness.GetInfo(userName);
            ViewData["userName"] = userName;
            ViewData["lstRoles"] = AccountBusiness.GetListUserRole();
            return View(accInfo);
        }

        #endregion

        #region Update Info
        [HttpPost]
        public object UpdateInfo(AccountInfo data)
        {
            var reponse = AccountBusiness.UpdateInfo(data);
            return reponse;
        }

        #endregion
    }
}
