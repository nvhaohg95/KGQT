using KGQT.Business;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT
{
    public class HomeController : Controller
    {
        //Dashboard
        [HttpGet]
        public ActionResult Dashboard()
        {
            var sUserName = HttpContext.Session.GetString("user");
            if(!string.IsNullOrEmpty(sUserName))
            {
                var user = Accounts.GetUserLogin(sUserName);
                return View(user);
            }
            return RedirectToAction("login","auth");
        }

        
    }
}
