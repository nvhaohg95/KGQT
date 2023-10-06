using KGQT.Business;
using KGQT.Commons;
using KGQT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NToastNotify;
using System.Reflection;

namespace KGQT.Controllers
{

    public class ShippingOrderController : Controller
    {
        #region constructor
        private IToastNotification _toastNotification;
        private static ShippingOrder _shippingBus;
        private static Packages _packages;
        public ShippingOrderController(IToastNotification toastNotification)
        {
            _toastNotification = toastNotification;
            _shippingBus = new ShippingOrder();
            _packages = new Packages();
        }
        #endregion

        #region View
        // GET: ShippingOrderController
        public ActionResult Index()
        {
            var lst = _shippingBus.GetList("", "", 0, 0);
            return View(lst);
        }

        // GET: ShippingOrderController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ShippingOrderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: ShippingOrderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
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

        // GET: ShippingOrderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
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
            return _packages.CheckExist(package);
        }
        #endregion
    }
}
