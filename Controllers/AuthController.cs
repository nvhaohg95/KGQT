using KGQT.Business;
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
                    if(result.Type == 1)
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
        public ActionResult ForgotPassword(string id,string tk)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string userName = Helper.Base64Decode(id);
                string token = Helper.Base64Decode(tk);
                var acc = Accounts.GetByUsernameAndToken(userName, token);
                if(acc != null)
                {
                    var data = new ForgotPassWord();
                    data.UserName = acc.Username;
                    data.Email = acc.Email;
                    return View(data);
                }
            } 
            return View();
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
        [HttpGet]
        public ActionResult SendMailForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SendMailForgetPassword(string email)
        {
            var result = await Accounts.SendMailForgetPassword(_mailSettings, email);
            if (result.IsError)
            {
                if (result.Type == 1)
                {
                    ModelState.AddModelError("error", result.Message);
                }
                else
                {
                    _toastNotification.AddInfoToastMessage(result.Message);
                }
                return View();
            }
            else
            {
                _toastNotification.AddSuccessToastMessage(result.Message);
                return Redirect("sendMailForgetPassword");
            }
        }
        #endregion

        #region Unites
        public async Task<ActionResult> Test()
        {
            return Ok("TEST");
        }
        #endregion
    }
}
