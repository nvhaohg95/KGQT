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
        private static NamespaceHandling _db;
        public ShippingOrderController(IToastNotification toastNotification)
        {
            _toastNotification = toastNotification;
        }

        #endregion

        #region View
        [HttpGet]
        public ActionResult Index(int status, string ID, DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            using (var db = new nhanshiphangContext())
            {
                var oData = ShippingOrder.GetList(status, ID, fromDate, toDate, page);
                var lstData = oData[0] as List<tbl_ShippingOrder>;
                decimal total = (decimal)oData[1];
                decimal totalPage = (decimal)oData[2];
                @ViewData["page"] = page;
                @ViewData["total"] = total;
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
        [HttpGet]
        public bool CheckPackage(string package)
        {
            return Packages.CheckExist(package);
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
        public JsonResult Create(tbl_ShippingOrder form, string package, string declares)
        {
            try
            {
                var userLogin = HttpContext.Session.GetString("user");
                var user = Accounts.GetInfo(-1, form.Username);
                form.Status = 1;
                form.CreatedDate = DateTime.Now;
                form.CreatedBy = userLogin;
                form.Username = user.Username;
                form.Email = user.Email;
                form.LastName = user.LastName;
                form.FirstName = user.FirstName;
                form.Phone = user.Phone;
                form.Address = user.Address;
                form.ShippingMethodName = PJUtils.ShippingMethodName(form.ShippingMethod.Value);
                var id = BusinessBase.Add(form, "ShippingOrderCode");
                if (id > -1 && !string.IsNullOrEmpty(package))
                {
                    var packs = package.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    List<tbl_Package> obj = new List<tbl_Package>();
                    foreach (var item in packs)
                    {

                        var p = new tbl_Package();
                        p.PackageCode = item;
                        p.Status = 1;
                        p.MovingMethod = id;
                        p.CreatedBy = user.Username;
                        p.CreatedDate = DateTime.Now;
                        BusinessBase.Add(p);
                    }
                    if (form.IsInsurance == true && !string.IsNullOrEmpty(declares))
                    {
                        var lstDeclare = JsonConvert.DeserializeObject<List<tbl_ShippingOrderDeclaration>>(declares);
                        var config = BusinessBase.GetFirst<tbl_Configuration>();
                        double totalPrice = 0;
                        bool save = false;
                        foreach (var d in lstDeclare)
                        {
                            d.ShippingOrderID = id;
                            d.ShippingOrderCode = form.ShippingOrderCode;
                            d.PriceVND = d.ProductQuantity * d.ProductQuantity * Convert.ToDouble(config.Currency);
                            d.CreatedBy = user.Username;
                            d.CreatedDate = DateTime.Now;
                            save = BusinessBase.Add(d);
                            if (save)
                                totalPrice += d.PriceVND.Value;
                        }
                        if (save)
                        {
                            double feeInsur = totalPrice * 0.05;
                            var ship = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.ID == id);
                            if (ship != null)
                            {
                                ship.InsurancePrice = feeInsur;
                                BusinessBase.Update(ship);
                            }
                        }
                    }
                    return Json(id);
                }
                return Json(id);
            }
            catch (Exception ex)
            {
                return Json(0);
            }
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
        public bool DeleteDeclare(int id)
        {
            var entity = BusinessBase.GetOne<tbl_ShippingOrderDeclaration>(x => x.ID == id);
            var s = BusinessBase.Remove<tbl_ShippingOrderDeclaration>(entity);
            if (!s)
                return false;

            var order = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.ID == entity.ShippingOrderID);
            var lstDeclare = BusinessBase.GetList<tbl_ShippingOrderDeclaration>(x => x.ShippingOrderID == order.ID);
            var feeInsur = lstDeclare.Sum(x => x.PriceVND);

            order.InsurancePrice = feeInsur * 0.05;
            order.ModifiedBy = HttpContext.Session.GetString("user");
            order.ModifiedDate = DateTime.Now;
            if (BusinessBase.Update(order))
            {
                var sUser = HttpContext.Session.GetString("US_LOGIN");
                if (!string.IsNullOrEmpty(sUser))
                {
                    var user = JsonConvert.DeserializeObject<UserLogin>(sUser);
                    BusinessBase.TrackLog(user.ID, order.ID, "{0} đã xóa kê khai đơn" + id, 0, user.Username);
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

            var oUser = BusinessBase.GetOne<tbl_Account>(x => x.Username == username);

            if (oUser == null) return false;

            bool check = oUser.Wallet > oOrder.TotalPrice;
            if (check)
            {
                var pay = oUser.Wallet - oOrder.TotalPrice;

                oOrder.Status = 2;
                oOrder.ModifiedBy = username;
                oOrder.ModifiedDate = DateTime.Now;

                var s = BusinessBase.Update(oOrder);
                if (s)
                {
                    BusinessBase.TrackLog(oUser.ID, oOrder.ID, "{0} đã thanh toán cho đơn hàng {1}", 1, username);

                    oUser.Wallet = pay;
                    BusinessBase.Update(oUser);

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

                        BusinessBase.TrackLog(oUser.ID, pack.ID, "{0} đã thanh toán cho kiện {1}", 1, username);
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
