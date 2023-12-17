using ExcelDataReader;
using Fasterflect;
using KGQT.Base;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MimeKit;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class PackageController : Controller
    {
        #region View
        public IActionResult Index(int status, string ID, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            var oData = Packages.GetPage(status, ID, fromDate, toDate, page, pageSize);
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

        public IActionResult Edit(int id)
        {
            var o = BusinessBase.GetOne<tbl_Package>(x => x.ID == id);
            return View(o);
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
        public IActionResult PackagePartial(string code)
        {
            var package = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode.ToLower() == code.ToLower());
            return PartialView("_Package", package);
        }

        [HttpGet]
        public IActionResult InStockPartial(string code)
        {
            var package = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode.ToLower() == code.ToLower());
            return PartialView("_InStock", package);
        }

        [HttpGet]
        public IActionResult DownloadFile(string fileName)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(fileName);
            return File(fileBytes, MimeTypes.GetMimeType(fileName), Path.GetFileName(fileName));
        }

        [HttpGet]
        public IActionResult QueryOrderStatus(string code)
        {
            var user = HttpContext.Session.GetString("user");
            var data = Packages.GetStatusOrder(code, user);
            return View(data);
        }

        #endregion

        #region Function
        /// <summary>
        /// Tạo Mã vận đơn - 1 mã
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Create(string sData)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var data = JsonConvert.DeserializeObject<tbl_Package>(sData);
            var oSave = Packages.Add(data, userLogin);
            return Json(oSave);
        }


        /// <summary>
        /// Tạo mã vận đơn theo file excel
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        [HttpPost]
        public object CreateWithFile(IFormFile file, string sheet)
        {
            var oData = new DataReturnModel<object>();
            if (file == null)
            {
                Log.Error("Tạo file với excel,Csv", "Không có file dc chọn");
                oData.IsError = true;
                oData.Message = "Không có file nào được chọn!";
                return oData;
            }
            var userLogin = HttpContext.Session.GetString("user");

            oData = Packages.CreateWithFileExcel(file, sheet, userLogin);
            return oData;
        }


        /// <summary>
        /// Kiểm tra kiện có tồn tại không
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        [HttpGet]
        public bool CheckPackage(string package)
        {
            return Packages.CheckExist(package);
        }

        /// <summary>
        /// Cập nhật trạng thái của kiện theo file excel (tạm thời k sử dụng nữa)
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public object ExportChinaWareHouse(string sheet, IFormFile file)
        {
            if (file == null)
            {
                Log.Error("Cập nhật xuất kho China", "Không có file dc chọn");
                return new { status = -1 };
            }
            var userLogin = HttpContext.Session.GetString("user");
            var oData = Packages.ExportChinaWareHouse(file, sheet, userLogin);
            return oData;
        }


        /// <summary>
        /// Cập nhật trạng thái của kiện
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public bool Update(tempPackage form)
        {
            var crrUse = HttpContext.Session.GetString("user");

            var oSave = Packages.Update(form, crrUse);
            return oSave;
        }

        /// <summary>
        /// Gợi ý tên khách hàng
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AutoComplete(string s)
        {
            var data = BusinessBase.GetList<tbl_Account>(x => x.Username.Contains(s)).ToList();
            return Json(data);
        }

        /// <summary>
        /// Nhập kho
        /// </summary>
        /// <param name="id"></param>
        /// <param name="weight"></param>
        /// <param name="woodPrice"></param>
        /// <param name="airPrice"></param>
        /// <returns></returns>
        [HttpPost]
        public bool InStockPackage(int id, string username, int moving, double weight, double woodPrice, double airPrice)
        {
            var crrUse = HttpContext.Session.GetString("user");
            var oSave = Packages.InStockHCMWareHouse(id, username, moving, weight, woodPrice, airPrice, crrUse);
            return oSave;
        }


        /// <summary>
        /// Hủy bỏ đơn
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public bool Cancel(int id)
        {
            var p = BusinessBase.GetOne<tbl_Package>(x => x.ID == id);
            if (p != null)
            {
                p.Status = 9;
                p.ModifiedBy = HttpContext.Session.GetString("user");
                p.ModifiedDate = DateTime.Now;
                return BusinessBase.Update(p);
            }
            return false;
        }
        #endregion
    }
}
