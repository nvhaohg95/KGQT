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


        // GET: ShippingOrderController/Create
        public ActionResult Create()
        {
            return View();
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
                    HistoryPayWallet.Insert(oUser.ID, oUser.Username, oOrder.ID, "", oOrder.TotalPrice.Value, 1, 1, pay.Value, username);
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

        // POST: ShippingOrderController/Create
        /// <summary>
        /// Status: 1: chưa xác nhận,2:Hàng về kho TQ, 3:Đang trên đường về HCM,
        /// 4:Hàng về tới HCM, 5:Đã nhận hàng,9:Đã hủy, 10: Thất lạc, 11: không nhận dc hàng
        /// </summary>
        /// <param name="form"></param>
        /// <param name="package"></param>
        /// <param name="declares"></param>
        /// <returns></returns>
        [HttpPost]
        public bool Create(tbl_ShippingOrder form, string package, string declares)
        {
            try
            {
                if (string.IsNullOrEmpty(package)) return false;

                var userLogin = HttpContext.Session.GetString("user");
                var user = AccountBusiness.GetInfo(-1, form.Username);
                form.Status = 1;
                form.CreatedDate = DateTime.Now;
                form.CreatedBy = userLogin;
                form.Username = user.Username;
                form.Email = user.Email;
                form.LastName = user.LastName;
                form.FirstName = user.FirstName;
                form.Phone = user.Phone;
                form.Address = user.Address;
                form.Weight = 0;
                form.WeightPrice = 0;
                form.InsurancePrice = 0;
                form.AirPackagePrice = 0;
                form.WoodPackagePrice = 0;
                form.TotalPrice = 0;
                form.ShippingMethodName = PJUtils.ShippingMethodName(form.ShippingMethod.Value);
                var sPacks = package.Split(';', StringSplitOptions.RemoveEmptyEntries);
                var oPacks = BusinessBase.GetList<tbl_Package>(x => sPacks.Contains(x.PackageCode));

                foreach (var item in oPacks)
                {
                    if (item.IsInsurance != null && item.IsInsurance == true)
                    {
                        form.IsInsurance = true;
                        form.InsurancePrice += item.IsInsurancePrice;
                    }

                    if (item.IsWoodPackage != null && item.IsWoodPackage == true)
                    {
                        form.IsWoodPackage = true;
                        form.WoodPackagePrice += item.WoodPackagePrice;
                    }

                    if (item.IsAirPackage != null && item.IsAirPackage == true)
                    {
                        form.IsAirPackage = true;
                        form.AirPackagePrice += item.AirPackagePrice;
                    }

                    form.WeightPrice += item.WeightPrice;
                    form.Weight += item.Weight;

                }

                var totalPrice = form.WeightPrice + form.AirPackagePrice + form.AirPackagePrice + form.InsurancePrice;
                form.TotalPrice = totalPrice;
                var s = BusinessBase.Add(form);
                return s;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
