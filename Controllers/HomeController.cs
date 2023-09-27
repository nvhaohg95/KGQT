using KGQT.Business;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NToastNotify;
using System.Xml.Linq;

namespace KGQT
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;
        private IToastNotification _toastNotification;

        public IToastNotification NotificationService
        {
            get { return _toastNotification; }
            set { _toastNotification = value; }
        }



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
            var sUser = HttpContext.Session.GetString("US_LOGIN");
            UserLogin user = null;
            if (!string.IsNullOrEmpty(sUser))
            {
                user = JsonConvert.DeserializeObject<UserLogin>(sUser);
                if (user != null)
                    return View();
            }
            return RedirectToAction("login", "auth");

        }

        #endregion

        #region Change Password
        [HttpPost]
        public JsonResult ChangePassword(ChangePassword data)
        {
            var result = Accounts.ChangePassword(data);
            if (result.IsError)
            {
                NotificationService.AddErrorToastMessage(result.Message);
            }
            else
            {
                NotificationService.AddSuccessToastMessage(result.Message);
                HttpContext.Session.Remove("US_LOGIN");
            }
            return Json(result);
        }

        #endregion

        [HttpPost]
        public JsonResult Test(ChangePassword data)
        {
            if (ModelState.IsValid)
            {
                NotificationService.AddSuccessToastMessage("Success");
            }
            else
                NotificationService.AddErrorToastMessage("Error");

            return Json(data);
        }



    }
}
