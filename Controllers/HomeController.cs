using KGQT.Business;
using KGQT.Business.Base;
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

        [HttpGet]
        public ActionResult Dashboard()
        {
            var username = HttpContext.Session.GetString("user");
            var pack = Packages.GetAllStatus(username);
            ViewData["pack_st3"] = pack[0];
            ViewData["pack_st4"] = pack[1];
            ViewData["pack_st5"] = pack[2];

            var ship = ShippingOrder.GetAllStatus(username);
            ViewData["order_st1"] = ship[0];
            ViewData["order_st2"] = ship[1];
            ViewData["ordertotal"] = ship[2];
            return View();

        }

        #endregion


    }


}
