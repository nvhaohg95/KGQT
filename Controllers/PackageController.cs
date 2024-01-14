using KGQT.Base;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Controllers
{
    public class PackageController : Controller
    {
        #region View
        public IActionResult Index(int status, string ID, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 10)
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
            var user = BusinessBase.GetOne<tbl_Account>(x => x.ID == model.UID);
            model.Username = user.Username;
            model.Phone = user.Phone;
            model.Address = user.Address;
            model.Email = user.Email;
            return View(model);
        }

        [HttpGet]
        public IActionResult QueryOrderStatus(string code)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var data = Packages.GetStatusOrder(code, userLogin);
            return View(data);
        }
        #endregion

        #region CRUD
        [HttpPost]
        public DataReturnModel<bool> Create(tbl_Package form)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var data = Packages.CustomerAdd(form,userLogin);
            return data;
        }

        #endregion

    }
}
