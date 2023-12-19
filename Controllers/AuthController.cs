﻿using KGQT.Business;
using KGQT.Commons;
using KGQT.Mobility;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NToastNotify;
using Org.BouncyCastle.Tls;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace KGQT.Controllers
{

    public class AuthController : Controller
    {
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
            _toastNotification = toastNotification;
            _hostingEnvironment = hostingEnvironment;
            _mailSettings = mailSettings.Value;
        }


        #region Login
        [HttpGet]
        public ActionResult Login()
        {
            var sUser = HttpContext.Session.GetString("US_LOGIN");
            if (!string.IsNullOrEmpty(sUser))
            {
                var user = JsonConvert.DeserializeObject<tbl_Account>(sUser);
                if (user != null)
                {
                    if (user.RoleID == 0)
                        return RedirectToAction("Admin");
                    else
                        return RedirectToAction("Dashboard", "Home");
                }
            }
            return View();
        }
        [HttpPost]
        public DataReturnModel<tbl_Account> Login(UserModel model)
        {
            if(model == null)
            {
                return new DataReturnModel<tbl_Account>() { IsError = true, Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!" };
            }    
            var result = AccountBusiness.Login(model.UserName, model.PassWord);
            if (!result.IsError)
            {
                HttpContext.Session.SetString("user", model.UserName);
                HttpContext.Session.SetString("US_LOGIN", JsonConvert.SerializeObject(result.Data));
            }
            return result;
        }
        //[HttpPost]
        //public ActionResult Login(UserModel model)
        //{
        //    var result = _auth.Login(model.UserName,model.PassWord);
        //    if (result.IsError)
        //    {
        //        return View(model);
        //    }
        //    HttpContext.Session.SetString("user", model.UserName);
        //    HttpContext.Session.SetString("US_LOGIN", JsonConvert.SerializeObject(result.Data));
        //    if (result.Data != null)
        //    {
        //        if (result.Data.RoleID == 0)
        //            return RedirectToAction("admin");
        //    }
        //    return RedirectToAction("Dashboard", "Home");
        //}
        #endregion

        #region Logout
        public ActionResult Logout()
        {
            HttpContext.Session.Remove("US_LOGIN");
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
        public DataReturnModel<tbl_Account> Register(SignUpModel data)
        {
            if (data.File != null)
            {
                data.Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads", "avatars");
            }
            var result = AccountBusiness.Register(data);
            return result;
        }
        #endregion

        #region Change Password
        [HttpPost]
        public JsonResult ChangePassword(ChangePassword data)
        {
            var result = AccountBusiness.ChangePassword(data);
            if (!result.IsError)
            {
                HttpContext.Session.Remove("US_LOGIN");
                HttpContext.Session.Remove("user");
            }
            return Json(result);
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
                    var acc = AccountBusiness.GetByUsernameAndToken(userName, token);
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
                var result = AccountBusiness.ForgotPassword(data);
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
            var result = await AccountBusiness.SendMailForgetPassword(_mailSettings, email);
            return Json(result);
        }

        #endregion

    }
}
