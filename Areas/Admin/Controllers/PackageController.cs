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
        public List<tbl_Package> ScanPackage(string str)
        {
            return BusinessBase.GetList<tbl_Package>(x => x.PackageCode.Contains(str)).ToList();
        }

        #region Function
        [HttpPost]
        public async Task<bool> ImportChinaWareHouseAsync()
        {
            var file = Request.Form.Files[0];
            List<ExcelModel> content = PJUtils.ExcelToJson(file);
            var userLogin = HttpContext.Session.GetString("user");
            bool s = Packages.UpdateStatusCNWH(content, 1, userLogin);
            return s;
        }

        [HttpPost]
        public async Task<bool> EmportChinaWareHouseAsync()
        {
            var file = Request.Form.Files[0];
            List<ExcelModel> content = PJUtils.ExcelToJson(file);
            var userLogin = HttpContext.Session.GetString("user");
            bool s = Packages.UpdateStatusCNWH(content, 2, userLogin);
            return s;
        }
    }

    #endregion
}
