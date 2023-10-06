﻿using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NToastNotify;
using Org.BouncyCastle.Asn1.Cms;
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
            model.Packs = BusinessBase.GetList<tbl_Package>(x => x.ShippingOrderID == id);
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
        // POST: ShippingOrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tbl_ShippingOrder form)
        {
            try
            {
                var model = new tbl_ShippingOrder();
                return View();// RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        [HttpGet]
        public bool CheckPackage(string package)
        {
            return Packages.CheckExist(package);
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
        #endregion
    }
}
