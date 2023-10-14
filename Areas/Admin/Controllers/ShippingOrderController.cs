using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NToastNotify;
using System.Xml;

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
        // GET: ShippingOrderController
        [HttpGet]
        public ActionResult Index()
        {
            var lst = ShippingOrder.GetList(0, null, null, "");
            return View(lst);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(string searchText)
        {
            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tmpShippingOrder>();
                var shippingOrders = new List<tbl_ShippingOrder>();
                var query = db.tbl_ShippingOrders.AsQueryable();
                if (!string.IsNullOrEmpty(searchText))
                {
                    ViewData["serchText"] = searchText;
                    query = query.Where(x => x.Username.ToLower().Contains(searchText.ToLower()));
                }
                shippingOrders = query.ToList();
                if (shippingOrders.Count > 0)
                {
                    foreach (var item in shippingOrders)
                    {
                        var data = new tmpShippingOrder()
                        {
                            ID = item.ID,
                            ShippingOrderCode = item.ShippingOrderCode,
                            CreatedDate = item.CreatedDate != null ? item.CreatedDate.Value.ToShortDateString() : "",
                            DateExpectation = item.DateExpectation != null ? item.DateExpectation.Value.ToShortDateString() : "",
                            CreatedBy = item.CreatedBy,
                            Username = item.Username,
                            TotalPrice = item.TotalPrice,
                            Status = item.Status,
                            StatusName = item.Status == 1 ? "Chưa giao" : "Đã giao"
                        };
                        lstData.Add(data);
                    }
                }
                return View("Index", lstData);
            }
        }

        [HttpPost]
        public ActionResult Filter(int? status, string? timeSets, DateTime? fromDate, DateTime? toDate)
        {
            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tmpShippingOrder>();
                var shippingOrders = new List<tbl_ShippingOrder>();
                var query = db.tbl_ShippingOrders.AsQueryable();
                ViewData["Predicate"] = null;
                if (status != 0)
                {
                    query = query.Where(x => x.Status == status);
                    ViewData["Predicate"] = "Trạng thái: " + (status == 1 ? "Chưa giao" : "Đã giao");
                }

                if (fromDate != null && toDate != null)
                {
                    query = query.Where(x => x.CreatedDate >= fromDate && x.CreatedDate <= toDate);
                    if (ViewData["Predicate"] != null)
                    {
                        ViewData["Predicate"] += ", ";
                    }
                    ViewData["Predicate"] += "Khoảng thời gian: " + fromDate.Value.ToString("dd/MM/yyyy") + " - " + toDate.Value.ToString("dd/MM/yyyy");

                }
                shippingOrders = query.ToList();
                if (shippingOrders.Count > 0)
                {
                    foreach (var item in shippingOrders)
                    {
                        var data = new tmpShippingOrder()
                        {
                            ID = item.ID,
                            ShippingOrderCode = item.ShippingOrderCode,
                            CreatedDate = item.CreatedDate != null ? item.CreatedDate.Value.ToShortDateString() : "",
                            DateExpectation = item.DateExpectation != null ? item.DateExpectation.Value.ToShortDateString() : "",
                            CreatedBy = item.CreatedBy,
                            Username = item.Username,
                            TotalPrice = item.TotalPrice,
                            Status = item.Status,
                            StatusName = item.Status == 1 ? "Chưa giao" : "Đã giao"
                        };
                        lstData.Add(data);
                    }
                }
                return View("index", lstData);
            }
        }

        // GET: ShippingOrderController/Details/5
        public ActionResult Details(int id)
        {
            var model = new OrderDetails();
            model.Order = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.ID == id);
            model.Packs = BusinessBase.GetList<tbl_Package>(x => x.TransID == id);
            model.Declarations = BusinessBase.GetList<tbl_ShippingOrderDeclaration>(x => x.ShippingOrderID == id);
            return View(model);
        }

        // POST: ShippingOrderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        // POST: ShippingOrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
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
        /// Status: 0: chưa xác nhận, 1: Đã cập nhật mã vận đơn, 2:Hàng về kho TQ, 3:Đang trên đường về HCM,
        /// 4:Hàng về tới HCM, 5:Đã nhận hàng,9:Đã hủy, 10: Thất lạc, 1: không nhận dc hàng
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
                                ship.IsInsurancePrice = feeInsur;
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
                    BusinessBase.TrackLogShippingOrder(user.ID, order.ID, "{0} đã xác nhận đơn", 0, user.Username);
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
                    BusinessBase.TrackLogShippingOrder(user.ID, order.ID, "{0} đã hủy đơn", 0, user.Username);
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

            order.IsInsurancePrice = feeInsur * 0.05;
            order.ModifiedBy = HttpContext.Session.GetString("user");
            order.ModifiedDate = DateTime.Now;
            if (BusinessBase.Update(order))
            {
                var sUser = HttpContext.Session.GetString("US_LOGIN");
                if (!string.IsNullOrEmpty(sUser))
                {
                    var user = JsonConvert.DeserializeObject<UserLogin>(sUser);
                    BusinessBase.TrackLogShippingOrder(user.ID, order.ID, "{0} đã xóa kê khai đơn" + id, 0, user.Username);
                    return true;
                }

            }
            return false;
        }
        #endregion
    }
}
