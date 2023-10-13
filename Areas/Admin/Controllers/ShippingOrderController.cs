using KGQT.Areas.Admin.Models.temp;
using KGQT.Business;
using KGQT.Models;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Org.BouncyCastle.Asn1.Cms;
using System.Xml;

namespace KGQT.Areas.Admin.Controllers
{
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
            var lst = ShippingOrder.GetList("", "", 0, 0);
            return View(lst);
        }

        [HttpPost]
        public ActionResult Index(int status)
        {
            var lst = ShippingOrder.GetList("", "", 0, 0);
            return View("Index",lst);
        }




        // GET: ShippingOrderController/Details/5
        public ActionResult Details(int id)
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
            return Packages.CheckExist(package);
        }
        #endregion
    }
}
