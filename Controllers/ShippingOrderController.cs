using DocumentFormat.OpenXml.Office2010.Excel;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace KGQT.Controllers
{

    public class ShippingOrderController : Controller
    {
        #region constructor

        public ShippingOrderController()
        {

        }
        #endregion

        #region View
        // GET: ShippingOrderController
        public ActionResult Index(int status, string ID, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var oData = ShippingOrder.GetPage(status, ID, fromDate, toDate, page, pageSize, userLogin);
            var (total, totalLeft) = ShippingOrder.GetTotalAndLeft(status, ID, fromDate, toDate, userLogin);
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
            ViewBag.total = total;
            ViewBag.totalLeft = totalLeft;
            return View(lstData);
        }

        // GET: ShippingOrderController/Details/5
        public ActionResult Details(int id, string recID)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";

            var model = new OrderDetails();
            if (string.IsNullOrEmpty(recID))
                model.Order = ShippingOrder.GetOne(id);
            else
                model.Order = ShippingOrder.GetOne(recID);
            
            if (model.Order != null)
            {

                if (model.Order.Username.ToLower() != userLogin.ToLower())
                {
                    model = null;
                    return View(model);
                }

                model.Packs = PackagesBusiness.GetByTransId(model.Order.RecID);
                model.User = BusinessBase.GetOne<tbl_Account>(x => x.Username == model.Order.Username);
            }
            return View(model);
        }

        // GET: ShippingOrderController/Create
        public ActionResult Create()
        {
            return View();
        }
        #endregion

        #region Functions
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
        public object PaymentSelected(string data)
        {
            DataReturnModel<bool> ret = new DataReturnModel<bool>();
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            if (userModel == null)
            {
                ret.IsError = true;
                ret.Message = "Bạn chưa đăng nhập hoặc phiên đăng nhập đã hết hạn";
                return ret;
            }
            ret = ShippingOrder.PaymentSelected(data, userModel.UserName);
            return ret;
        }

        [HttpPost]
        public object PaymentAll()
        {
            DataReturnModel<bool> ret = new DataReturnModel<bool>();
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            if (userModel == null)
            {
                ret.IsError = true;
                ret.Message = "Bạn chưa đăng nhập hoặc phiên đăng nhập đã hết hạn";
                return ret;
            }
            ret = ShippingOrder.PaymentAll(userModel.UserName);
            return ret;
        }
        #endregion
    }
}
