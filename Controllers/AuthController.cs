using KGQT.Business;
using KGQT.Commons;
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
                var user = JsonConvert.DeserializeObject<UserLogin>(sUser);
                if (user != null)
                {
                    if(user.RoleID == 1)
                        return RedirectToAction("admin", "home");
                    return RedirectToAction("dashboard", "home");
                }
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
            HttpContext.Session.SetString("US_LOGIN", JsonConvert.SerializeObject(result.Data));
            if ((result.Data as UserLogin).RoleID == 1)
                return RedirectToAction("home", "admin");
            return RedirectToAction("dashboard", "home");
            //var result = new DataReturnModel() { };
            //result.Data = new UserLogin() { Username = "loc", FirstName = "Lộc", LastName = "Trần", IMG = "" };
            //HttpContext.Session.SetString("US_LOGIN", JsonConvert.SerializeObject(result.Data));
            //return RedirectToAction("home", "admin");
        }
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
