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
using NToastNotify;
using System.Data;
using System.Drawing.Printing;
using System.Xml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShippingOrderController : Controller
    {
        #region constructor
        private IToastNotification _toastNotification;
        public ShippingOrderController(IToastNotification toastNotification)
        {
            this._toastNotification = toastNotification;
        }

        #endregion

        #region View
        [HttpGet]
        public ActionResult Index(int status, string ID, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20)
        {
            var username = HttpContext.Request.Query["username"];
            using (var db = new nhanshiphangContext())
            {
                var oData = ShippingOrder.GetPage(status, ID, fromDate, toDate, page, pageSize, username);
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

            model.Packs = PackagesBusiness.GetByTransId(model.Order.RecID);
            if (model.Order != null)
                model.User = BusinessBase.GetOne<tbl_Account>(x => x.Username == model.Order.Username);
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
            order.Status = 1;
            order.ModifiedBy = HttpContext.Session.GetString("user");
            order.ModifiedDate = DateTime.Now;
            if (BusinessBase.Update(order))
            {
                var sUser = HttpContext.Session.GetString("US_LOGIN");
                if (!string.IsNullOrEmpty(sUser))
                {
                    var user = JsonConvert.DeserializeObject<UserLogin>(sUser);
                    return true;
                }

            }
            return false;
        }

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

            var oUser = BusinessBase.GetOne<tbl_Account>(x => x.Username == oOrder.Username);

            if (oUser == null) return new { error = true, mssg = "Không tìm thấy thông tin khách hàng" };
            double totalPrice = Convert.ToDouble(oOrder.TotalPrice);
            bool check = Convert.ToDouble(oUser.Wallet) >= totalPrice;
            if (check)
            {
                oOrder.Status = 2;
                oOrder.ModifiedBy = username;
                oOrder.ModifiedDate = DateTime.Now;

                var s = BusinessBase.Update(oOrder);
                if (s)
                {
                    oUser = BusinessBase.GetOne<tbl_Account>(x => x.UserID == oUser.UserID);
                    var pay = Converted.StringCeiling(Converted.ToDouble(oUser.Wallet) - totalPrice);

                    if (!AccountBusiness.UpdateWallet(oUser.ID, pay))
                        return new { error = true, mssg = "Thanh toán thất bại !" };

                    HistoryPayWallet.Insert(oUser.ID, oUser.Username, oOrder.ID, "", oOrder.TotalPrice, 1, 1, pay, username);

                    var trade = new tbl_TradeHistory();
                    trade.UID = oUser.ID;
                    trade.Title = PJUtils.TradeName(1);
                    trade.OrderCode = oOrder.ShippingOrderCode;
                    trade.OrderPrice = Converted.Double2Money(totalPrice);
                    trade.AmountIn = "0";
                    trade.AmountDebt = "0";
                    trade.PaymentMethod = 1;
                    trade.PayCode = Guid.NewGuid().ToString("N");
                    trade.Status = 1;
                    trade.CreatedDate = DateTime.Now;
                    trade.CreatedBy = "Auto";
                    BusinessBase.Add(trade);

                    var packs = BusinessBase.GetList<tbl_Package>(x => x.TransID == oOrder.ShippingOrderCode);
                    foreach (var pack in packs)
                    {
                        pack.Status = 5;
                        pack.ModifiedBy = username;
                        pack.ModifiedDate = DateTime.Now;
                        BusinessBase.Update(pack);

                        BusinessBase.TrackLog(oUser.ID, pack.ID, "{0} đã thanh toán cho kiện {1}", 1, oOrder.Username);
                    }
                    return new { error = false };
                }
            }
            else
            {
                var pay = Converted.StringCeiling(totalPrice - Converted.ToDouble(oUser.Wallet));
                return new { error = true, mssg = "Tài khoản khách không đủ tiền, cần phải nạp thêm " + Converted.String2Money(pay)+"đ" };
            }
            return false;
        }


        [HttpPost]
        public object receiver(int id)
        {
            if (id == 0) return false;
            var oOrder = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.ID == id);
            if (oOrder == null) return new { error = true, mssg = "Không tìm thấy thông tin đơn" };

            if (oOrder.Status == 2) return new { error = true, mssg = "Đơn này đã thanh toán rồi" };

            var username = HttpContext.Session.GetString("user");

            oOrder.Status = 2;
            oOrder.ModifiedBy = username;
            oOrder.ModifiedDate = DateTime.Now;

            var s = BusinessBase.Update(oOrder);
            if (s)
            {
                var packs = BusinessBase.GetList<tbl_Package>(x => x.TransID == oOrder.ShippingOrderCode);
                foreach (var pack in packs)
                {
                    pack.Status = 5;
                    pack.ModifiedBy = username;
                    pack.ModifiedDate = DateTime.Now;
                    BusinessBase.Update(pack);
                }
            }
            return new { error = false };
        }
        #endregion
    }
}
