using KGQT.Base;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace KGQT.Controllers
{
    public class PackageController : Controller
    {
        #region View
        public IActionResult Index(int status, string ID, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 2)
        {
            var username = HttpContext.Session.GetString("user");
            var oData = Packages.GetPage(status, ID, fromDate, toDate, page, pageSize, username);
            var lstPackage = oData[0];
            int totalRecord = (int)oData[1];
            int totalPage = (int)oData[2];
            ViewData["status"] = status;
            ViewData["page"] = page;
            ViewData["totalRecord"] = totalRecord;
            ViewData["totalPage"] = totalPage;
            ViewData["ID"] = ID;
            ViewData["fromDate"] = fromDate;
            ViewData["toDate"] = toDate;
            return View(lstPackage);
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
        public bool Create(tbl_Package form)
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
            return s;
        }

        [HttpPost]
        public bool Test()
        {
            return true;
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
