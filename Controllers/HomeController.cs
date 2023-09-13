using KGQT.Business;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NToastNotify;

namespace KGQT
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;
        private IToastNotification _toastNotification;

        #region Contructor
        public HomeController(IConfiguration configuration, IToastNotification toastNotification)
        {
            _configuration = configuration;
            _toastNotification = toastNotification;
        }
        #endregion


        #region Home
        [HttpGet]
        public ActionResult Dashboard()
        {
            var sUserName = HttpContext.Session.GetString("user");
            if (!string.IsNullOrEmpty(sUserName))
            {
                var user = Accounts.GetUserLogin(sUserName);
                ViewData.Add("User",user);
                return View();
            }
            return RedirectToAction("login", "auth");
        }
        #endregion

        #region Change Password
        [HttpGet]
        public ActionResult ChangePassword()
        {
            var sUserName = HttpContext.Session.GetString("user");
            if (!string.IsNullOrEmpty(sUserName))
            {
                var user = Accounts.GetUserLogin(sUserName);
                var data = new ChangePassword();
                data.UserName = user.Username;
                ViewData.Add("User", user);
                return PartialView(data);
            }
            return PartialView();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassword data)
        {
            if (ModelState.IsValid)
            {
                var result = Accounts.ChangePassword(data);
                if(result.IsError)
                {
                    if(result.Type == 1)
                    { 
                        ModelState.AddModelError(result.Key,result.Message);
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage(result.Message);
                    }
                }
                else
                {
                    _toastNotification.AddSuccessToastMessage(result.Message);
                }
                HttpContext.Session.Remove("user");
                return RedirectToAction("login", "auth");
            }
            return View();
        }
        #endregion


    }
}
