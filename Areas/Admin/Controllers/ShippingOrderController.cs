using DocumentFormat.OpenXml.Drawing.Charts;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

using System.Data;
using System.Drawing.Printing;
using System.Xml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShippingOrderController : Controller
    {
        #region View
        [HttpGet]
        public ActionResult Index(int status, string ID, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            using (var db = new nhanshiphangContext())
            {
                var oData = ShippingOrder.GetPage(status, ID, fromDate, toDate, page, pageSize);
                var lstData = oData[0] as List<tbl_ShippingOrder>;
                int numberRecord = (int)oData[1];
                int numberPage = (int)oData[2];
                ViewBag.status = status;
                ViewBag.ID = ID;
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.pageCurrent = page;
                ViewBag.numberPage = numberPage;
                ViewBag.numberRecord = numberRecord;
                return View(lstData);
            }
        }


        // GET: ShippingOrderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: ShippingOrderController/Details/5
        public ActionResult Details(int id, string recID)
        {
            var model = new OrderDetails();
            if (string.IsNullOrEmpty(recID))
                model.Order = ShippingOrder.GetOne(id);
            else model.Order = ShippingOrder.GetOne(recID);
            if (model.Order != null)
            {
                model.Packs = PackagesBusiness.GetByTransId(model.Order.RecID);
                model.User = BusinessBase.GetOne<tbl_Account>(x => x.Username.ToLower() == model.Order.Username.ToLower());
            }
            return View(model);
        }

        #endregion

        #region Functions
        [HttpGet]
        public IActionResult AutoCompletePackage(string s)
        {
            var data = BusinessBase.GetList<tbl_Package>(x => x.PackageCode.Contains(s)).ToList();
            return Json(data);
        }


        [HttpPost]
        public bool Confirm(int id)
        {
            var order = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.ID == id);
            if (order == null)
                return false;
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            order.Status = 1;
            order.ModifiedBy = userLogin;
            order.ModifiedDate = DateTime.Now;
            return BusinessBase.Update(order);
        }

        [HttpPost]
        public object Cancel(int id)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var dtRetunr = ShippingOrder.Cancel(id, userLogin);
            return dtRetunr;
        }

        [HttpPost]
        public object Payment(int id)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            DataReturnModel<bool> ret = ShippingOrder.Payment(id, userLogin);
            return ret;
        }

        [HttpPost]
        public object PaymentDirect(int id)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            DataReturnModel<bool> ret = ShippingOrder.PaymentDirect(id, userLogin);
            return ret;
        }


        [HttpPost]
        public object receiver(int id)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            DataReturnModel<bool> ret = ShippingOrder.Receiver(id, userLogin);
            return ret;
        }
        
        [HttpPost]
        public bool NotifiOrder()
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var ret = ShippingOrder.NotifiOrder(userLogin);
            return ret;
        }
        #endregion
    }
}
