using KGQT.Business;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Controllers
{
 
    public class AuthController : Controller
    {
        IConfiguration _configuration;
        KGNewContext _db;   
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _db = new KGNewContext();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        #region Login
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
            return RedirectToAction("login","auth");
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
            if (!ModelState.IsValid)
            {
                data.CraetedDate = DateTime.Now;
                data.CraetedBy = data.UserName;
                var result = Accounts.RegisterAccount(data);
                if (result.IsError)
                {
                    ModelState.AddModelError("error", result.Message);
                    return View();
                }
                ViewBag.Alert = "Tạo tài khoảng thành công!";
                return RedirectToAction("Login");
            }
            if (data.Gender != 1 && data.Gender != 2)
            {
                ModelState.AddModelError("Gender", "Vui lòng chọn giới tính");
            }
            return View();
        }
        #endregion
    }
}
