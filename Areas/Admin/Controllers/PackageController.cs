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
            List<ExcelModel> content = PJUtils.ReadExcelToJson(file);
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
        public bool SubmitPack(int id, double weight, double woodPrice, double airPrice)
        {
            var crrUse = HttpContext.Session.GetString("user");
            var pack = BusinessBase.GetOne<tbl_Package>(x => x.ID == id);
            if (pack == null) return false;
            var username = pack.Username;

            //Update tien package
            var lstFee = BusinessBase.GetList<tbl_FeeWeight>(x => x.Type == pack.MovingMethod);
            if (lstFee == null || lstFee.Count == 0) return false;

            var fee = lstFee.FirstOrDefault(x => x.WeightFrom <= weight && weight <= x.WeightTo);

            if (lstFee == null || lstFee.Count == 0) return false;
            if (weight > fee.MinWeight)
                pack.Weight = weight;
            else
                pack.Weight = fee.MinWeight;
            pack.WeightReal = weight;
            pack.WoodPackagePrice = woodPrice;
            pack.AirPackagePrice = airPrice;
            pack.TotalPrice = pack.WeightPrice + airPrice + woodPrice + Convert.ToDouble(pack.IsInsurance);
            pack.Status = 3;
            pack.ModifiedBy = crrUse;
            pack.ModifiedDate = DateTime.Now;
            var p = BusinessBase.Update(pack);
            if (p)
            {
                BusinessBase.TrackLog(pack.UID.Value, pack.ID, "{0} đã nhập kho kiện với cân năng " + weight + "kg", 0, crrUse);

                if (pack.IsAirPackage.HasValue && pack.IsAirPackage == true)
                {
                    BusinessBase.TrackLog(pack.UID.Value, pack.ID, "{0} đã nhập kho kiện với giá quấn bọt khí " + airPrice + "đ", 0, crrUse);
                }
                if (pack.IsWoodPackage.HasValue && pack.IsWoodPackage == true)
                {
                    BusinessBase.TrackLog(pack.UID.Value, pack.ID, "{0} đã nhập kho kiện với giá đóng gỗ " + woodPrice + "đ", 0, crrUse);
                }

                //Ktra xem hom nay khach da co don chua.
                DateTime startDate = DateTime.Now.Date; //One day 
                DateTime endDate = startDate.AddDays(1).AddTicks(-1);
                var check = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.Username == username && x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.ShippingMethod == pack.MovingMethod);
                if (check != null)
                {

                    //Cap nhat lai tranid cho package
                    pack.TransID = check.ID;
                    pack.ModifiedBy = crrUse;
                    pack.ModifiedDate = DateTime.Now;
                    BusinessBase.Update(pack);
                    BusinessBase.TrackLog(pack.UID.Value, check.ID, "{0} đã cập nhật mã vận đơn " + check.ID + " vào kiện", 0, crrUse);

                    var lstPack = BusinessBase.GetList<tbl_Package>(x => x.TransID == check.ID);
                    double totalWeight = lstPack.Where(x => x.Weight != null).Sum(x => x.Weight.Value);
                    double totalWeightPrice = 0;

                    var finalfee = lstFee.FirstOrDefault(x => x.WeightFrom <= totalWeight && totalWeight <= x.WeightTo);
                    if (finalfee != null)
                    {
                        totalWeightPrice = totalWeight * finalfee.Amount.Value;
                    }

                    double totalWoodPrice = lstPack.Where(x => x.WoodPackagePrice != null).Sum(x => x.WoodPackagePrice.Value);
                    double totalAirPrice = lstPack.Where(x => x.AirPackagePrice != null).Sum(x => x.AirPackagePrice.Value);
                    double totalInsurPrice = lstPack.Where(x => x.IsInsurancePrice != null).Sum(x => x.IsInsurancePrice.Value);

                    var totalPrice = totalWeightPrice + totalWoodPrice + totalAirPrice + totalInsurPrice;
                    check.Weight = totalWeight;
                    check.WeightPrice = totalWeightPrice;
                    check.WoodPackagePrice = totalWoodPrice;
                    check.AirPackagePrice = totalAirPrice;
                    check.InsurancePrice = totalInsurPrice;
                    check.TotalPrice = totalPrice;
                    check.ModifiedBy = crrUse;
                    check.ModifiedDate = DateTime.Now;
                    var oUpdate = BusinessBase.Update(check);
                    BusinessBase.TrackLog(pack.UID.Value, check.ID, "{0} đã thêm kiện " + pack.PackageCode + " vào đơn", 0, crrUse);
                }
                else
                {

                    var ship = new tbl_ShippingOrder();
                    ship.ShippingOrderCode = pack.PackageCode;
                    ship.Username = pack.Username;
                    ship.FirstName = pack.FullName;
                    ship.LastName = pack.FullName;
                    ship.Email = pack.Email;
                    ship.Address = pack.Address;
                    ship.Phone = pack.Phone;
                    ship.ShippingMethod = pack.MovingMethod;
                    ship.ShippingMethodName = PJUtils.ShippingMethodName(pack.MovingMethod);
                    ship.Weight = pack.Weight;
                    ship.WeightPrice = pack.Weight * fee.Amount;
                    ship.IsAirPackage = pack.IsAirPackage;
                    ship.AirPackagePrice = pack.AirPackagePrice;
                    ship.IsWoodPackage = pack.IsWoodPackage;
                    ship.WoodPackagePrice = pack.WoodPackagePrice;
                    ship.IsInsurance = pack.IsInsurance;
                    ship.InsurancePrice = pack.IsInsurancePrice;
                    ship.TotalPrice = ship.WeightPrice.Value + ship.AirPackagePrice + ship.WoodPackagePrice + ship.InsurancePrice;
                    ship.Status = pack.Status;
                    ship.CreatedDate = DateTime.Now;
                    ship.CreatedBy = crrUse;
                    var oAdd = BusinessBase.Add(ship);
                    if (p)
                    {
                        var oPack = BusinessBase.GetOne<tbl_Package>(x => x.ID == pack.ID);
                        if (oPack != null)
                        {
                            oPack.TransID = ship.ID;
                            oPack.ModifiedBy = crrUse;
                            oPack.ModifiedDate = DateTime.Now;
                            BusinessBase.Update(oPack);
                            BusinessBase.TrackLog(oPack.UID.Value, oPack.ID, "{0} đã cập nhật mã vận đơn " + ship.ID + " vào kiện", 0, crrUse);

                        }
                        BusinessBase.TrackLog(pack.UID.Value, ship.ID, "{0} đã tạo đơn " + ship.ID + " với kiện " + pack.PackageCode + " vào đơn", 0, crrUse);
                    }

                }
            }
            return p;
        }
        #endregion
    }
}
