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
        private KGNewContext _db;
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
            _db = new KGNewContext();
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
                HttpContext.Session.Remove("user");
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
