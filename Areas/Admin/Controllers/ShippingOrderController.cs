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
            using (var db = new nhanshiphangContext())
            {
                var oData = ShippingOrder.GetPage(status, ID, fromDate, toDate, page, pageSize);
                var lstData = oData[0] as List<tbl_ShippingOrder>;
                int totalRecord = (int)oData[1];
                int totalPage = (int)oData[2];
                @ViewData["status"] = status;
                @ViewData["ID"] = ID;
                @ViewData["fromDate"] = fromDate;
                @ViewData["toDate"] = toDate;
                @ViewData["page"] = page;
                @ViewData["totalRecord"] = totalRecord;
                @ViewData["totalPage"] = totalPage;
                return View(lstData);
            }
        }

        // GET: ShippingOrderController/Details/5
        public ActionResult Details(int id)
        {
            var model = new OrderDetails();
            model.Order = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.ID == id);
            model.Packs = BusinessBase.GetList<tbl_Package>(x => x.TransID == id);
            return View(model);
        }
        #endregion

        #region Functions
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
                    BusinessBase.TrackLog(user.ID, order.ID, "{0} đã xác nhận đơn", 0, user.Username);
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
                    BusinessBase.TrackLog(user.ID, order.ID, "{0} đã hủy đơn", 0, user.Username);
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
            if (oOrder == null) return false;

            var username = HttpContext.Session.GetString("user");

            var oUser = BusinessBase.GetOne<tbl_Account>(x => x.Username == oOrder.Username);

            if (oUser == null) return false;

            bool check = oUser.Wallet > oOrder.TotalPrice;
            if (check)
            {
                oOrder.Status = 2;
                oOrder.ModifiedBy = username;
                oOrder.ModifiedDate = DateTime.Now;

                var s = BusinessBase.Update(oOrder);
                if (s)
                {
                    oUser = BusinessBase.GetOne<tbl_Account>(x => x.UserID == oUser.UserID);
                    var pay = oUser.Wallet - oOrder.TotalPrice;

                    oUser.Wallet = pay;
                    BusinessBase.Update(oUser);

                    #region Logs
                    BusinessBase.TrackLog(oUser.ID, oOrder.ID, "{0} đã thanh toán cho đơn hàng {1}", 0, oOrder.Username);
                    HistoryPayWallet.Insert(oUser.ID, oUser.Username, oOrder.ID, oOrder.TotalPrice.Value, 1, 1, pay.Value);
                    #endregion

                    var trade = new tbl_TradeHistory();
                    trade.UID = oUser.ID;
                    trade.Title = PJUtils.TradeName(1);
                    trade.OrderCode = oOrder.ShippingOrderCode;
                    trade.OrderPrice = string.Format("{0:N0}đ", oOrder.TotalPrice).Replace(",", ".");
                    trade.AmountIn = "0";
                    trade.AmountDebt = "0";
                    trade.PaymentMethod = 1;
                    trade.PayCode = Guid.NewGuid().ToString("N");
                    trade.Status = 1;
                    trade.CreatedDate = DateTime.Now;
                    trade.CreatedBy = "Auto";
                    BusinessBase.Add(trade);

                    var packs = BusinessBase.GetList<tbl_Package>(x => x.TransID == id);
                    foreach (var pack in packs)
                    {
                        pack.Status = 5;
                        pack.ModifiedBy = username;
                        pack.ModifiedDate = DateTime.Now;
                        BusinessBase.Update(pack);

                        BusinessBase.TrackLog(oUser.ID, pack.ID, "{0} đã thanh toán cho kiện {1}", 1, oOrder.Username);
                    }
                    return new { res = false };
                }
            }
            else
            {
                var pay = oOrder.TotalPrice - oUser.Wallet;
                return new { error = true, mssg = string.Format("{0:N0}đ", pay).Replace(",", ".") };
            }
            return false;
        }
        #endregion
    }
}
