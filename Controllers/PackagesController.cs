using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Controllers
{
    public class PackagesController : Controller
    {
        #region View
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("user");
            var model = BusinessBase.GetList<tbl_Package>(x => x.Username == username);
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var model = BusinessBase.GetOne<tbl_Package>(x => x.ID == id);
            return View(model);
        }
        #endregion

        #region CRUD
        [HttpPost]
        public JsonResult Create(tbl_Package form)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var user = Accounts.GetInfo(-1, userLogin);
            form.Status = 0;
            form.UID = user.ID;
            form.Username = user.Username;
            form.FullName = user.FirstName + " " + user.LastName;
            form.Phone = user.Phone;
            form.Email = user.Email;
            form.Address = user.Address;
            form.CreatedDate = DateTime.Now;
            form.CreatedBy = user.Username;

            if (form.IsInsurance.HasValue && form.IsInsurance == true)
            {
                form.IsInsurancePrice = form.DeclarePrice * 0.05;
            }

            var s = BusinessBase.Add(form);
            if (s)
            {
                BusinessBase.TrackLog(user.ID, form.ID, "{0} đã tạo kiện", 0, user.Username);
            }
            return Json(s);
        }
        #endregion

        #region Function
        [HttpGet]
        public bool CheckPackage(string package)
        {
            return Packages.CheckExist(package);
        }
        #endregion
    }
}
