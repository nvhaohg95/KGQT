using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NToastNotify;

namespace KGQT.Controllers
{

    public class ShippingOrderController : Controller
    {
        #region constructor
        private IToastNotification _toastNotification;
        public ShippingOrderController(IToastNotification toastNotification)
        {
            _toastNotification = toastNotification;
        }
        #endregion

        #region View
        // GET: ShippingOrderController
        public ActionResult Index(int status, string ID, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var oData = ShippingOrder.GetPage(status, ID, fromDate, toDate, page, pageSize, userLogin);
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

        // GET: ShippingOrderController/Details/5
        public ActionResult Details(int id)
        {
            var model = new OrderDetails();
            model.Order = ShippingOrder.GetOne(id);
            model.Packs = PackagesBusiness.GetByTransId(model.Order.RecID);
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
            string username = HttpContext.Session.GetString("user");
            var dtRetunr = ShippingOrder.Cancel(id, username);
            return dtRetunr;
        }

        [HttpPost]
        public object Payment(int id)
        {
            var username = HttpContext.Session.GetString("user");
            DataReturnModel<bool> ret = ShippingOrder.Payment(id, username);
            return ret;
        }
        #endregion
    }
}
