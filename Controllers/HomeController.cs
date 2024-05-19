using KGQT.Base;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using System.Xml.Linq;

namespace KGQT
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;

        #region Contructor
        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Home
        [HttpGet]
        public ActionResult TestGet([FromQuery] int a, [FromQuery] string b, [FromQuery] DateTime c)
        {
            return Ok(new { a, b, c });
        }
        public class ABC
        {
            public int A { get; set; }
            public string B { get; set; }
            public DateTime C { get; set; }
        }
        [HttpPost]
        public ActionResult TestPost(string data)
        {
            return Ok(data);
        }



        #endregion

        #region Dashboard
        [HttpGet]
        public ActionResult Dashboard()
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userName = userModel != null ? userModel.UserName : "";
            var pack = PackagesBusiness.GetAllStatus(userName);
            ViewData["pack_st3"] = pack[0];
            ViewData["pack_st4"] = pack[1];
            ViewData["pack_st5"] = pack[2];
            var ship = ShippingOrder.GetAllStatus(userName);
            ViewData["order_st1"] = ship[0];
            ViewData["order_st2"] = ship[1];
            ViewData["ordertotal"] = ship[2];
            return View();
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Policy()
        {
            return View();
        }

       
    }


}
