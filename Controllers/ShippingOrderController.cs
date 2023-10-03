using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NToastNotify;
using System.Dynamic;

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
        public ActionResult Details(int id)
        {
            var model = new OrderDetails();

            model.Order = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.ID == id);
            model.Packs = BusinessBase.GetList<tbl_Package>(x => x.ShippingOrderID == id);
            return View(model);
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
        public JsonResult Create(tbl_ShippingOrder form, string package, string declares)
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
                        p.ShippingOrderID = id;
                        p.CreatedBy = user.Username;
                        p.CreatedDate = DateTime.Now;
                        BusinessBase.Add(p);
                    }
                    if (form.IsInsurance == true && !string.IsNullOrEmpty(declares))
                    {
                        var lstDeclare = JsonConvert.DeserializeObject<List<tbl_ShippingOrderDeclaration>>(declares);
                        var config = BusinessBase.GetFirst<tbl_Configuration>();
                        double totalPrice = 0;
                        int save = -1;
                        foreach (var d in lstDeclare)
                        {
                            d.ShippingOrderID = id;
                            d.ShippingOrderCode = form.ShippingOrderCode;
                            d.PriceVND = d.ProductQuantity * d.ProductQuantity * Convert.ToDouble(config.Currency);
                            d.CreatedBy = user.Username;
                            d.CreatedDate = DateTime.Now;
                            save = BusinessBase.Add(d);
                            if (save > -1)
                                totalPrice += d.PriceVND.Value;
                        }
                        if (save > -1)
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


        [HttpGet]
        public bool CheckPackage(string package)
        {
            return Packages.CheckExist(package);
        }
        #endregion
    }
}
