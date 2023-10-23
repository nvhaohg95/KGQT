using ExcelDataReader;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
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
            var package = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode.ToLower() == code.ToLower());
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

        [HttpGet]
        public IActionResult GetAccount(string s)
        {
            var data = BusinessBase.GetList<tbl_Account>(x => x.Username.Contains(s)).ToList();
            return Json(data);
        }

        [HttpPost]
        public IActionResult SubmitPack(int id, double weight, double woodPrice, double airPrice)
        {
            var crrUse = HttpContext.Session.GetString("user");
            var pack = BusinessBase.GetOne<tbl_Package>(x => x.ID == id);
            if (pack == null) return null;
            var userId = pack.Username;

            //Update tien package
            var lstFee = BusinessBase.GetList<tbl_FeeWeight>(x => x.Type == pack.MovingMethod);
            if (lstFee == null || lstFee.Count == 0) return null;

            var fee = lstFee.FirstOrDefault(x => x.WeightFrom <= weight && weight <= x.WeightTo);

            if (lstFee == null || lstFee.Count == 0) return null;
            if (weight > fee.MinWeight)
                pack.Weight = weight;
            else
                pack.Weight = fee.MinWeight;
            pack.WeightReal = weight;
            pack.WeightPrice = weight * fee.Amount;
            pack.WoodPackagePrice = woodPrice;
            pack.AirPackagePrice = airPrice;
            pack.ModifiedBy = crrUse;
            pack.ModifiedDate = DateTime.Now;
            var p = BusinessBase.Update(pack);
            if (p)
            {
                BusinessBase.TrackLog(pack.UID.Value, pack.ID, "{0} đã nhập kho kiện với cân năng " + weight + "kg", 0, crrUse);

                if (pack.IsAirPackage.HasValue && pack.IsAirPackage == true)
                {
                    BusinessBase.TrackLog(pack.UID.Value, pack.ID, "{0} đã nhập kho kiện với giá quấn bọt khí " + airPrice + "kg", 0, crrUse);
                }
                if (pack.IsWoodPackage.HasValue && pack.IsWoodPackage == true)
                {
                    BusinessBase.TrackLog(pack.UID.Value, pack.ID, "{0} đã nhập kho kiện với giá đóng gỗ " + woodPrice + "kg", 0, crrUse);
                }

                //Ktra xem hom nay khach da co don chua.
                DateTime startDate = DateTime.Now.Date; //One day 
                DateTime endDate = startDate.AddDays(1).AddTicks(-1);
                var check = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.Username == userId && x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.ShippingMethod == pack.MovingMethod);
                if (check != null)
                {
                    var lstPack = BusinessBase.GetList<tbl_Package>(x => x.TransID == check.ID);
                    double totalWeight = lstPack.Where(x => x.Weight != null).Sum(x => x.Weight.Value);
                    double totalWeightPrice = lstPack.Where(x => x.WeightPrice != null).Sum(x => x.WeightPrice.Value);
                    double totalWoodPrice = lstPack.Where(x => x.WoodPackagePrice != null).Sum(x => x.WoodPackagePrice.Value);
                    double totalAirPrice = lstPack.Where(x => x.AirPackagePrice != null).Sum(x => x.AirPackagePrice.Value);
                    double totalInsurPrice = lstPack.Where(x => x.IsInsurancePrice != null).Sum(x => x.IsInsurancePrice.Value);

                    var totalPrice = totalWeightPrice + totalWoodPrice + totalAirPrice + totalInsurPrice;
                    check.Weight = totalWeight;
                    check.WeightPrice = totalWeightPrice;
                    check.WoodPackagePrice = totalWoodPrice;
                    check.AirPackagePrice = totalAirPrice;
                    check.InsurancePrice = totalInsurPrice;
                    check.ModifiedBy = crrUse;
                    check.ModifiedDate = DateTime.Now;
                    BusinessBase.Update(check);
                }
                else
                {
                    var ship = new tbl_ShippingOrder();


                }
            }
            return Json(new { id, weight, woodPrice, airPrice });
        }
        #endregion
    }
}
