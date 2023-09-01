using KGQT.Business;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Buffers.Text;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using static System.Net.Mime.MediaTypeNames;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace KGQT.Controllers
{
 
    public class AuthController : Controller
    {
        private IConfiguration _configuration;
        private KGNewContext _db;
        private IToastNotification _toastNotification;
        private readonly IHostingEnvironment _hostingEnvironment;

        public AuthController(IConfiguration configuration, IToastNotification toastNotification, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _db = new KGNewContext();
            _toastNotification = toastNotification;
            _hostingEnvironment = hostingEnvironment;
        }


        #region Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel model)
        {
            if (!string.IsNullOrEmpty(model.UserName) && !string.IsNullOrEmpty(model.Password) && ModelState.IsValid)
            {
                var result = Accounts.Login(model.UserName, model.Password);
                if (!result.IsError && result.Data != null)
                {
                    HttpContext.Session.SetString("user", model.UserName);
                    return RedirectToAction("dashboard","home");
                }
                ModelState.AddModelError("error", result.Message);
            }
            return View();
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
                if(data.File != null)
                {
                    data.Path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads","avatars");
                }
                var result = Accounts.RegisterAccount(data);
                if (result.IsError)
                {
                    this._toastNotification.AddWarningToastMessage(result.Message);
                    return View();
                }
                this._toastNotification.AddSuccessToastMessage(result.Message);
                return Redirect("login");

            }
            return View(data);
        }
        #endregion
    }
}
