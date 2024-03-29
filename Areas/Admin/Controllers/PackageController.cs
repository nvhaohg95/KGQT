﻿using ClosedXML.Excel;
using KGQT.Base;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Text;
namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class PackageController : Controller
    {
        #region View
        public IActionResult Index(int status, string ID, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            var oData = PackagesBusiness.GetPage(status, ID, fromDate, toDate, page, pageSize);
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

        public IActionResult ViewListFile()
        {
            string p = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExportExcel");
            List<FileInfoModel> listFile = new List<FileInfoModel>();
            FileService.LoadFiles(p, listFile);
            return View(listFile.OrderByDescending(x => x.CreatedOn).ToList());
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
            var user = BusinessBase.GetOne<tbl_Account>(x => x.ID == package.UID);
            if (user != null)
            {
                package.Username = user.Username;
                package.Phone = user.Phone;
                package.Address = user.Address;
                package.Email = user.Email;
            }
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
            var p = PackagesBusiness.GetInStock(code);
            return PartialView("_Package", p);
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
        public IActionResult ExportFile(int status, DateTime? fromDate, DateTime? toDate)
        {
            var data = PackagesBusiness.GetListExport(status, fromDate, toDate);
            @ViewData["status"] = status;
            @ViewData["fromDate"] = fromDate;
            @ViewData["toDate"] = toDate;
            return View(data.Data);
        }

        [HttpGet]
        public FileResult GenerateExcel(int status, DateTime? fromDate, DateTime? toDate)
        {
            var data = PackagesBusiness.GetListExport(status, fromDate, toDate);

            var dt = new DataTable("Excel");
            dt.Columns.AddRange(new DataColumn[]{
                new DataColumn("MVĐ"),
                new DataColumn("Username"),
                new DataColumn("Tuyến"),
                new DataColumn("Cân nặng",Type.GetType("System.Int32")),
                new DataColumn("Ngày xuất kho")
            });

            foreach (var item in data.Data)
            {
                var type = PJUtils.MovingMethod2Type(item.MovingMethod);
                dt.Rows.Add(item.PackageCode, item.Username, type, item.WeightReal, item.ExportedCNWH);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Export" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx");
                }
            }
        }

        [HttpGet]
        public IActionResult QueryOrderStatus(string code)
        {
            var user = HttpContext.Session.GetString("user");
            var data = PackagesBusiness.GetStatusOrder(code, user);
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
            var oSave = PackagesBusiness.Add(data, userLogin);
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
            oData = PackagesBusiness.CreateWithFileExcel(file, sheet, userLogin);
            return oData;
        }

        /// <summary>
        /// type 1= dong go, 2=bot khi, 3=bao hiem; 4 = hang hieu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        [HttpPost]
        public bool UpdateService(int id, int type, bool check)
        {
            var pack = PackagesBusiness.GetOne(id);
            if(pack == null)
            return false;

            if(type == 1)
                pack.IsWoodPackage = check;
            if(type==2)
                pack.IsAirPackage = check;
            if(type == 3)
                pack.IsInsurance = check;
            if (type == 4)
                pack.IsBrand = check;
            return BusinessBase.Update(pack);
        }


        /// <summary>
        /// Kiểm tra kiện có tồn tại không
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        [HttpGet]
        public bool CheckPackage(string package)
        {
            return PackagesBusiness.CheckExist(package);
        }

        /// <summary>
        /// Cập nhật trạng thái của kiện
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public bool Update(tbl_Package form)
        {
            var crrUse = HttpContext.Session.GetString("user");

            var oSave = PackagesBusiness.Update(form, crrUse);
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
        /// Gợi ý file lô
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult BigPackageComplete(string s)
        {
            var data = BusinessBase.GetList<tbl_BigPackage>(x => x.ID.ToString() == s || x.BigPackageCode.Contains(s)).ToList();
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
        public bool InStockPackage(string sData)
        {
            var crrUse = HttpContext.Session.GetString("user");
            if (string.IsNullOrEmpty(sData)) return false;
            var data = JsonConvert.DeserializeObject<tmpInStock>(sData);
            var oSave = PackagesBusiness.InStockHCMWareHouse(data, crrUse);
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

        public void ExportFile(IFormFile file, List<tempExport> data)
        {
            PJUtils.MarkupValueExcel(file, file.FileName, data);
        }
        #endregion
    }
}
