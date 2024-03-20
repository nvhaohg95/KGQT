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
            model.Packs = PackagesBusiness.GetByTransId(model.Order.ShippingOrderCode);
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
        public bool Cancel(int id)
        {
            var order = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.ID == id);
            if (order == null)
                return false;
            order.Status = 9;
            order.ModifiedBy = HttpContext.Session.GetString("user");
            order.ModifiedDate = DateTime.Now;
            if (BusinessBase.Update(order))
            {
                var sUser = HttpContext.Session.GetString("US_LOGIN");
                if (!string.IsNullOrEmpty(sUser))
                {
                    var user = JsonConvert.DeserializeObject<UserLogin>(sUser);
                    BusinessBase.TrackLog(user.ID, order.ID, "{0} đã hủy đơn", 1, user.Username);
                    return true;
                }

            }
            return false;
        }

        [HttpPost]
        public object Payment(int id)
        {
            if (id == 0) return false;
            var oOrder = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.ID == id);
            if (oOrder == null) return new { error = true, mssg = "Không tìm thấy thông tin đơn" };

            if (oOrder.Status == 2) return new { error = true, mssg = "Đơn này đã thanh toán rồi" };

            var username = HttpContext.Session.GetString("user");
            if (username != oOrder.Username) return false;
            var oUser = BusinessBase.GetOne<tbl_Account>(x => x.Username == username);

            if (oUser == null) return new { error = true, mssg = "Không tìm thấy thông tin khách hàng" };
            double totalPrice = Converted.ToDouble(oOrder.TotalPrice);
            bool check = oUser.Wallet > totalPrice;
            if (check)
            {
                oOrder.Status = 2;
                oOrder.ModifiedBy = username;
                oOrder.ModifiedDate = DateTime.Now;

                var s = BusinessBase.Update(oOrder);
                if (s)
                {
                    oUser = BusinessBase.GetOne<tbl_Account>(x => x.UserID == oUser.UserID);
                    var pay = oUser.Wallet - totalPrice;
                    oUser.Wallet = pay;
                    BusinessBase.Update(oUser);

                    #region Logs
                    BusinessBase.TrackLog(oUser.ID, oOrder.ID, "{0} đã thanh toán cho đơn hàng {1}", 1, oOrder.Username);
                    HistoryPayWallet.Insert(oUser.ID, oUser.Username, oOrder.ID, "", totalPrice, 1, 1, pay.Value, username);
                    #endregion

                    var packs = BusinessBase.GetList<tbl_Package>(x => x.TransID == oOrder.ShippingOrderCode);
                    foreach (var pack in packs)
                    {
                        pack.Status = 5;
                        pack.ModifiedBy = username;
                        pack.ModifiedDate = DateTime.Now;
                        BusinessBase.Update(pack);

                        BusinessBase.TrackLog(oUser.ID, pack.ID, "{0} đã thanh toán cho kiện {1}", 1, username);
                    }
                    return new { error = false };
                }
            }
            else
            {
                var pay = totalPrice - oUser.Wallet;
                return new { error = true, mssg = "Tài khoản không đủ tiền, cần phải nạp thêm " + Converted.Double2Money(pay) };
            }
            return false;
        }
        #endregion
    }
}
