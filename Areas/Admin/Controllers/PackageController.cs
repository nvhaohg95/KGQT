using ExcelDataReader;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Reflection.Metadata;
using System.Text;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class PackageController : Controller
    {
        public IActionResult Index()
        {
            var model = Packages.GetPage();
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var package = BusinessBase.GetOne<tbl_Package>(x => x.ID == id);
            return View(package);
        }

        [HttpGet]
        public IActionResult InStock(string str)
        {
            var model = BusinessBase.GetList<tbl_Package>(x => x.PackageCode.Contains(str)).ToList();
            return View(model);
        }
        [HttpGet]
        public IActionResult PackagePartial(/*DateTime date, int method,M*/ string code)
        {
            var package = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode.ToLower() ==code.ToLower());
            //if (!string.IsNullOrEmpty(code))
            //{
            //    DateTime start = date;
            //    DateTime end = start.AddDays(1).AddTicks(-1);
            //    packages = BusinessBase.GetList<tbl_Package>(x => x.PackageCode.EndsWith(code)
            //    && x.MovingMethod == method
            //    && x.ExportedCNWH >= start && x.ExportedCNWH < end).ToList();
            //}
            return PartialView("_Package", package);
        }

        #region Function
        [HttpPost]
        public async Task<bool> EmportChinaWareHouseAsync()
        {
            var file = Request.Form.Files[0];
            List<ExcelModel> content = PJUtils.ExcelToJson(file);
            var userLogin = HttpContext.Session.GetString("user");
            bool s = Packages.UpdateStatusCNWH(content, 2, userLogin);
            return s;
        }

        [HttpPost]
        public JsonResult Create(tbl_Package form)
        {
            var userLogin = HttpContext.Session.GetString("user");

            var user = Accounts.GetInfo(-1, form.Username);
            form.Status = 0;
            form.UID = user.ID;
            form.Username = user.Username;
            form.FullName = user.FirstName + " " + user.LastName;
            form.Phone = user.Phone;
            form.Email = user.Email;
            form.Address = user.Address;
            form.CreatedDate = DateTime.Now;
            form.CreatedBy = userLogin;
            return Json(BusinessBase.Add(form));
        }

        [HttpGet]
        public IActionResult GetAccount(string s)
        {
            var data = BusinessBase.GetList<tbl_Account>(x => x.Username.Contains(s)).ToList();
            return Json(data);
        }
        #endregion
    }
}
