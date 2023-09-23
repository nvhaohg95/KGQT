﻿using KGQT.Business;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NToastNotify;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace KGQT.Controllers
{

    public class AuthController : Controller
    {
        private IConfiguration _configuration;
        private KGNewContext _db;
        private IToastNotification _toastNotification;
        private readonly IHostingEnvironment _hostingEnvironment;
        private MailSettings _mailSettings;

        public IToastNotification NotificationService
        {
            get { return _toastNotification; }
            set { _toastNotification = value; }
        }
        public AuthController(IConfiguration configuration, IToastNotification toastNotification, IHostingEnvironment hostingEnvironment, IOptions<MailSettings> mailSettings)
        {
            _configuration = configuration;
            _db = new KGNewContext();
            _toastNotification = toastNotification;
            _hostingEnvironment = hostingEnvironment;
            _mailSettings = mailSettings.Value;
        }


        #region Login
        [HttpGet]
        public ActionResult Login()
        {
            var sUserName = HttpContext.Session.GetString("user");
            if (!string.IsNullOrEmpty(sUserName))
            {
                return RedirectToAction("dashboard", "home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel data)
        {
            var result = Accounts.Login(data.UserName, data.PassWord);
            if (result.IsError)
            {
                ModelState.AddModelError(result.Key, result.Message);
                return View(data);
            }
            HttpContext.Session.SetString("user", data.UserName);
            return RedirectToAction("dashboard", "home");
        }
        #endregion

        #region Logout
        public ActionResult Logout()
        {
            HttpContext.Session.Remove("user");
            return Redirect("login");
        }
        #endregion

        #region Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(SignUpModel data)
        {
            if (ModelState.IsValid)
            {
                if (data.File != null)
                {
                    data.Path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "avatars");
                }
                var result = Accounts.RegisterAccount(data);
                if (result.IsError)
                {
                    if (result.Type == 1)
                        ModelState.AddModelError(result.Key, result.Message);
                    else
                        _toastNotification.AddWarningToastMessage(result.Message);
                    return View();
                }
                _toastNotification.AddSuccessToastMessage(result.Message);
                return Redirect("login");

            }
            return View();
        }
        #endregion

        #region Forgot Pasword
        [HttpGet]
        public ActionResult ForgotPassword(string id, string tk)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(tk))
            {
                string userName = Helper.Base64Decode(id);
                string strToken = Helper.Base64Decode(tk);
                var token = strToken.Split("|")[0];
                var time = DateTime.Parse(strToken.Split("|")[1]);
                if (DateTime.Now.Subtract(time).TotalMinutes <= 30)
                {
                    var acc = Accounts.GetByUsernameAndToken(userName, token);
                    if (acc != null)
                    {
                        var data = new ForgotPassWord();
                        data.UserName = acc.Username;
                        data.Email = acc.Email;
                        return View(data);
                    }
                }
            }
            return RedirectToAction("index", "error");
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotPassWord data)
        {
            if (ModelState.IsValid)
            {
                var result = Accounts.ForgotPassword(data);
                if (result.IsError)
                {
                    if (result.Type == 1)
                        ModelState.AddModelError(result.Key, result.Message);
                    else
                        _toastNotification.AddWarningToastMessage(result.Message);
                    return View();
                }
                _toastNotification.AddSuccessToastMessage(result.Message);
                return Redirect("login");
            }
            return View();
        }

        #endregion

        #region Send Mail Forget Password
        [HttpPost]
        public async Task<JsonResult> SendMailForgetPassword(string email)
        {
            var result = await Accounts.SendMailForgetPassword(_mailSettings, email);
            if (result.IsError)
                NotificationService.AddWarningToastMessage(result.Message);
            else
                NotificationService.AddSuccessToastMessage(result.Message);
            return Json(result);
        }

        #endregion

    }
}
