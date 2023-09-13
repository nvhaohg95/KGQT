using KGQT.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class ShippingOrderController : Controller
    {
        private static ShippingOrder _shippingBus = new ShippingOrder();
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

        // POST: ShippingOrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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
    }
}
