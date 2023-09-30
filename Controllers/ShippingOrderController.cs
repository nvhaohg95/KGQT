using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult Index()
        {
            var lst = ShippingOrder.GetList("", "", 0, 0);
            return View(lst);
        }

        // GET: ShippingOrderController/Details/5
        public IEnumerable<tbl_Package> packages { get; set; }
        public ActionResult Details(int id)
        {
            var ship = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.ID == id);
            packages = BusinessBase.GetList<tbl_Package>(x => x.ShippingOrderID == id);
            return View(ship);
        }

        // GET: ShippingOrderController/Create
        public ActionResult Create()
        {
            return View();
        }
        #endregion

        #region Functions
        // POST: ShippingOrderController/Create
        [HttpPost]
        public JsonResult Create(tbl_ShippingOrder form, string package)
        {
            try
            {
                var userLogin = HttpContext.Session.GetString("user");
                var user = Accounts.GetInfo(-1, userLogin);
                form.Status = 1;
                form.CreatedDate = DateTime.Now;
                form.CreatedBy = user.Username;
                form.Username = user.Username;
                form.Email = user.Email;
                form.LastName = user.LastName;
                form.FirstName = user.FirstName;
                form.Phone = user.Phone;
                form.Address = user.Address;
                form.ShippingMethodName = PJUtils.ShippingMethodName(form.ShippingMethod.Value);
                var save = BusinessBase.Add(form, "ShippingOrderCode");
                if (save > -1 && !string.IsNullOrEmpty(package))
                {
                    var packs = package.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    List<tbl_Package> obj = new List<tbl_Package>();
                    foreach (var item in packs)
                    {

                        var p = new tbl_Package();
                        p.PackageOrderHangCode = item;
                        p.Status = 1;
                        p.ShippingOrderID = save;
                        p.CreatedBy = user.Username;
                        p.CreatedDate = DateTime.Now;
                      save =  BusinessBase.Add(p);
                    }
                    return Json(save);
                }
                return Json(save);
            }
            catch (Exception ex)
            {
                return Json(0);
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
