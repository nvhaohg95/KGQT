using KGQT.Base;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO.Packaging;

namespace KGQT.Controllers
{
    public class PackageController : Controller
    {
        #region View
        public IActionResult Index(int status, string ID, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 10)
        {
            var username = HttpContext.Session.GetString("user");
            var oData = PackagesBusiness.GetPage(status, ID, fromDate, toDate, page, pageSize, username);
            var lstPackage = oData[0];
            int numberRecord = (int)oData[1];
            int numberPage = (int)oData[2];
            ViewBag.status = status;
            ViewBag.ID = ID;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            return View(lstPackage);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var model = BusinessBase.GetOne<tbl_Package>(x => x.ID == id);
            var user = BusinessBase.GetOne<tbl_Account>(x => x.ID == model.UID);
            if (user != null)
            {
                model.Username = user.Username;
                model.Phone = user.Phone;
                model.Address = user.Address;
                model.Email = user.Email;
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult QueryOrderStatus(string code)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var data = PackagesBusiness.GetStatusOrder(code, userLogin);
            return View(data);
        }
        #endregion

        #region CRUD
        [HttpPost]
        public DataReturnModel<bool> Create(tbl_Package form)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var data = PackagesBusiness.CustomerAdd(form,userLogin);
            return data;
        }

        public DataReturnModel<bool> Cancel(int id)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var data = PackagesBusiness.Cancel(id, userLogin);
            return data;
        }
        #endregion

    }
}
