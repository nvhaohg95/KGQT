using CsvHelper;
using CsvHelper.Configuration;
using ExcelDataReader;
using Fasterflect;
using KGQT.Base;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace KGQT.Business
{
    public static class Packages
    {
        #region Select
        public static object[] GetAllStatus(string username)
        {
            using (var db = new nhanshiphangContext())
            {
                IQueryable<tbl_Package> qry = db.tbl_Packages;
                if (!string.IsNullOrEmpty(username))
                    qry = qry.Where(x => x.Username == username);

                int st1 = qry.Where(x => x.Status == 3).Count();
                int st2 = qry.Where(x => x.Status == 4).Count();
                int st3 = qry.Where(x => x.Status == 5).Count();
                return new object[] { st1, st2, st3 };
            }
        }

        public static bool CheckExist(string package)
        {
            using (var db = new nhanshiphangContext())
                return db.tbl_Packages.Any(x => x.PackageCode == package);
        }

        public static object[] GetPage(int status, string ID, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 2, string userName = "")
        {
            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tbl_Package>();
                int count = 0;
                int totalPage = 0;
                IQueryable<tbl_Package> qry = db.tbl_Packages;
                if (status > 0)
                    qry = qry.Where(x => x.Status == status);
                if (!string.IsNullOrEmpty(ID))
                    qry = qry.Where(x => x.PackageCode.Contains(ID));
                if (fromDate != null)
                    qry = qry.Where(x => x.CreatedDate >= fromDate);
                if (toDate != null)
                    qry = qry.Where(x => x.CreatedDate <= toDate);
                if (!string.IsNullOrEmpty(userName))
                    qry = qry.Where(x => x.Username == userName);
                count = qry.Count();
                if (count > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)count / pageSize));
                    lstData = qry.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }
                return new object[] { lstData, count, totalPage };
            }
        }

        public static DataReturnModel<tmpChinaOrderStatus> GetStatusOrder(string code, string uslogin)
        {
            DataReturnModel<tmpChinaOrderStatus> data = new DataReturnModel<tmpChinaOrderStatus>();
            var oPack = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode == code && x.Status < 2);
            if (oPack != null)
            {
                var user = BusinessBase.GetOne<tbl_Account>(x => x.ID == oPack.UID);

                if (oPack.SearchBaiduTimes == null)
                {
                    oPack.SearchBaiduTimes = 0;
                    if (user == null || user.Wallet == null || user.Wallet < 500)
                    {
                        data.IsError = false;
                        data.Message = "Tài khoản khách không đủ tiền";
                        return data;
                    }
                    var wallet = user.Wallet - 500;
                    BusinessBase.Update(user);

                    #region Logs
                    HistoryPayWallet.Insert(user.ID, user.Username, oPack.ID, "Thanh toán tiền gọi api kiểm tra Mã Vận Đơn " + oPack.PackageCode, 500, 1, 1, wallet.Value, uslogin);
                    #endregion
                }

                using (HttpClient client = new HttpClient())
                {
                    string url = string.Format(Config.Settings.ApiUrl, Config.Settings.ApiKey, code);
                    var response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string sData = response.Content.ReadAsStringAsync().Result;
                        data.Data = JsonConvert.DeserializeObject<tmpChinaOrderStatus>(sData);
                    }
                    else
                    {
                        data.IsError = false;
                        data.Message = "Không thể kết nối đến server!";
                        return data;
                    }
                }

                oPack.SearchBaiduTimes++;
                BusinessBase.Update(oPack);
            }
            else
            {
                data.IsError = false;
                data.Message = "Đơn hàng chưa được tạo trên hệ thống tracking.nhanshiphang.vn";
                return data;
            }
            data.Key = code;
            return data;
        }
        #endregion

        #region CRUD
        public static bool Add(string sData, string userLogin)
        {
            var form = JsonConvert.DeserializeObject<tbl_Package>(sData);
            var user = AccountBusiness.GetInfo(-1, form.Username);
            form.Status = 0;
            form.PackageCode = form.PackageCode.Trim().Replace("\'", "").Replace(" ", "");
            form.UID = user.ID;
            form.Username = user.Username;
            form.FullName = user.FirstName + " " + user.LastName;
            form.Phone = user.Phone;
            form.Email = user.Email;
            form.Address = user.Address;
            form.OrderDate = DateTime.Now;
            form.CreatedDate = DateTime.Now;
            form.CreatedBy = userLogin;
            if (form.IsInsurance.HasValue && form.IsInsurance == true)
            {
                form.IsInsurancePrice = form.DeclarePrice * 0.05;
            }

            var s = BusinessBase.Add(form);
            if (s)
                BusinessBase.TrackLog(user.ID, form.ID, "{0} đã tạo kiện", 0, user.Username);
            return s;
        }
        public static DataReturnModel<bool> CustomerAdd(string sData, string userLogin)
        {
            var data = new DataReturnModel<bool>();
            var form = JsonConvert.DeserializeObject<tbl_Package>(sData);
            var user = BusinessBase.GetOne<tbl_Account>(x => x.Username == userLogin);
            form.PackageCode = form.PackageCode.Trim().Replace("\'", "").Replace(" ", "");

            var exist = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode == form.PackageCode && (x.UID == null || x.UID == user.ID));

            if (exist != null)
            {
                if (exist.UID == user.ID)
                {
                    exist.UID = user.ID;
                    exist.Username = user.Username;
                    exist.FullName = user.FullName;
                    exist.Address = user.Address;
                    exist.Phone = user.Phone;
                    exist.Email = user.Email;
                }
                if (exist.MovingMethod != form.MovingMethod)
                    exist.MovingMethod = form.MovingMethod;

                exist.IsAirPackage = form.IsAirPackage;
                exist.IsWoodPackage = form.IsWoodPackage;
                exist.IsInsurance = form.IsInsurance;
                if (form.IsInsurance.HasValue && form.IsInsurance == true)
                {
                    exist.Declaration = form.Declaration;
                    exist.DeclarePrice = form.DeclarePrice;
                    form.IsInsurancePrice = form.DeclarePrice * 0.05;
                }

                exist.ModifiedBy = user.Username;
                exist.ModifiedDate = DateTime.Now;
                var update = BusinessBase.Update(exist);
                if (update)
                    BusinessBase.TrackLog(user.ID, form.ID, "{0} đã cập nhật thông tin kiện", 0, user.Username);
                data.IsError = update;
                data.Message = "Mã vận đơn đã có trong hệ thống, vui lòng liên hệ với nhân viên để đối chiếu thông tin !";
                return data;
            }

            form.Status = 0;
            form.UID = user.ID;
            form.Username = user.Username;
            form.FullName = user.FullName;
            form.Phone = user.Phone;
            form.Email = user.Email;
            form.Address = user.Address;
            form.OrderDate = DateTime.Now;
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

        public static bool Update(tempPackage form, string userLogin)
        {
            var p = BusinessBase.GetOne<tbl_Package>(x => x.ID == form.ID);
            if (p == null) return false;
            int oldStt = p.Status;
            if (form.Username != p.Username)
            {
                var user = AccountBusiness.GetInfo(form.Username);
                if (user == null) return false;

                p.Username = user.UserName;
                p.UID = user.ID;
                p.FullName = user.FirstName + " " + user.LastName;
            }
            p.PackageCode = form.PackageCode;
            p.MovingMethod = form.MovingMethod;
            p.IsAirPackage = form.IsAirPackage;
            p.AirPackagePrice = form.AirPackagePrice;
            p.IsInsurancePrice = form.IsInsurancePrice;
            p.Declaration = form.Declaration;
            p.DeclarePrice = form.DeclarePrice;
            p.IsWoodPackage = form.IsWoodPackage;
            p.WoodPackagePrice = form.WoodPackagePrice;
            p.WareHouse = form.WareHouse;
            p.Status = form.Status;
            p.ModifiedBy = userLogin;
            p.ModifiedDate = DateTime.Now;
            if (oldStt != p.Status)
            {
                if (p.Status == 3)
                {
                    p.ExportedCNWH = DateTime.Now;
                    p.DateExpectation = Packages.CaclDateExpectation(p);
                    p.Exported = true;
                }
            }
            return BusinessBase.Update(form);
        }

        public static bool InStockHCMWareHouse(int id, string username, double weight, double woodPrice, double airPrice, string accessor)
        {
            var pack = BusinessBase.GetOne<tbl_Package>(x => x.ID == id);
            if (pack == null) return false;
            if (pack.Username != username)
            {
                var user = BusinessBase.GetOne<tbl_Account>(x => x.Username == username);
                pack.Username = username;
                pack.UID = user.ID;
                pack.FullName = user.FullName;
            }
            //Update tien p
            var config = BusinessBase.GetFirst<tbl_Configuration>();
            double minPackage = 0.3;
            double minOrder = 1;
            if (config != null)
            {
                minPackage = config.MinPackage ?? 0.3;
                minOrder = config.MinOrder ?? 1;
            }
            var lstFee = BusinessBase.GetList<tbl_FeeWeight>(x => x.Type == pack.MovingMethod);
            if (lstFee == null || lstFee.Count == 0) return false;

            var fee = lstFee.FirstOrDefault(x => x.WeightFrom <= weight && weight <= x.WeightTo);

            if (lstFee == null || lstFee.Count == 0) return false;
            if (weight > minPackage)
                pack.Weight = weight;
            else
                pack.Weight = minPackage;
            pack.WeightReal = weight;
            pack.WoodPackagePrice = woodPrice;
            pack.AirPackagePrice = airPrice;
            pack.Status = 4;
            pack.ModifiedBy = accessor;
            pack.ModifiedDate = DateTime.Now;
            pack.ImportedSGWH = DateTime.Now;
            var p = BusinessBase.Update(pack);
            if (p)
            {
                BusinessBase.TrackLog(pack.UID.Value, pack.ID, "{0} đã nhập kho kiện với cân năng " + weight + "kg", 0, accessor);

                if (pack.IsAirPackage.HasValue && pack.IsAirPackage == true)
                {
                    BusinessBase.TrackLog(pack.UID.Value, pack.ID, "{0} đã nhập kho kiện với giá quấn bọt khí " + airPrice + "đ", 0, accessor);
                }
                if (pack.IsWoodPackage.HasValue && pack.IsWoodPackage == true)
                {
                    BusinessBase.TrackLog(pack.UID.Value, pack.ID, "{0} đã nhập kho kiện với giá đóng gỗ " + woodPrice + "đ", 0, accessor);
                }

                //Ktra xem khach da co don chua.
                DateTime cnExportDateFrom = pack.ExportedCNWH.Value.Date;
                DateTime cnExportDateEnd = pack.ExportedCNWH.Value.AddDays(1).AddTicks(-1);
                DateTime startDate = DateTime.Now.Date; //One day 
                DateTime endDate = startDate.AddDays(1).AddTicks(-1);
                var check = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.Username == username && (x.CreatedDate >= startDate && x.CreatedDate <= endDate)
                                                                   && (x.ChinaExportDate >= cnExportDateFrom && x.ChinaExportDate <= cnExportDateEnd) && x.ShippingMethod == pack.MovingMethod);
                if (check != null)
                {
                    //Cap nhat lai tranid cho p
                    pack.TransID = check.ID;
                    pack.ModifiedBy = accessor;
                    pack.ModifiedDate = DateTime.Now;
                    BusinessBase.Update(pack);
                    BusinessBase.TrackLog(pack.UID.Value, check.ID, "{0} đã cập nhật mã vận đơn " + check.ID + " vào kiện", 0, accessor);

                    var lstPack = BusinessBase.GetList<tbl_Package>(x => x.TransID == check.ID);
                    double totalWeight = lstPack.Where(x => x.Weight != null).Sum(x => x.Weight.Value);
                    double totalWeightPrice = 0;

                    var finalfee = lstFee.FirstOrDefault(x => x.WeightFrom <= totalWeight && totalWeight <= x.WeightTo);
                    if (finalfee != null)
                    {
                        double weightRound = totalWeight;
                        if (totalWeight < minOrder)
                            weightRound = minOrder;
                        totalWeightPrice = weightRound * finalfee.Amount.Value;
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
                    check.ModifiedBy = accessor;
                    check.ModifiedDate = DateTime.Now;
                    var oUpdate = BusinessBase.Update(check);
                    BusinessBase.TrackLog(pack.UID.Value, check.ID, "{0} đã thêm kiện " + pack.PackageCode + " vào đơn", 0, accessor);
                }
                else
                {
                    var weightRound = pack.Weight;
                    double feeWeight = 0;
                    if (weightRound < 1)
                    {
                        feeWeight = (weightRound * fee.Amount).Value;
                    }
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
                    ship.WeightPrice = feeWeight;
                    ship.IsAirPackage = pack.IsAirPackage;
                    ship.AirPackagePrice = pack.AirPackagePrice ?? 0;
                    ship.IsWoodPackage = pack.IsWoodPackage;
                    ship.WoodPackagePrice = pack.WoodPackagePrice ?? 0;
                    ship.IsInsurance = pack.IsInsurance;
                    ship.InsurancePrice = pack.IsInsurancePrice ?? 0;
                    ship.TotalPrice = ship.WeightPrice.Value + ship.AirPackagePrice + ship.WoodPackagePrice + ship.InsurancePrice;
                    ship.Status = 1;
                    ship.ChinaExportDate = pack.ExportedCNWH;
                    ship.CreatedDate = DateTime.Now;
                    ship.CreatedBy = accessor;
                    var oAdd = BusinessBase.Add(ship);
                    if (p)
                    {
                        var oPack = BusinessBase.GetOne<tbl_Package>(x => x.ID == pack.ID);
                        if (oPack != null)
                        {
                            oPack.TransID = ship.ID;
                            oPack.ModifiedBy = accessor;
                            oPack.ModifiedDate = DateTime.Now;
                            BusinessBase.Update(oPack);
                            BusinessBase.TrackLog(oPack.UID.Value, oPack.ID, "{0} đã cập nhật mã vận đơn " + ship.ID + " vào kiện", 0, accessor);

                        }
                        BusinessBase.TrackLog(pack.UID.Value, ship.ID, "{0} đã tạo đơn " + ship.ID + " với kiện " + pack.PackageCode + " vào đơn", 0, accessor);
                    }
                }
            }
            return p;
        }
        public static DateTime CaclDateExpectation(tbl_Package item)
        {
            var dt = DateTime.Now;

            if (item.MovingMethod == 1)
            {
                return dt.AddDays(6);
            }

            if (item.MovingMethod == 2)
            {
                return dt.AddDays(10);
            }

            if (item.MovingMethod == 3)
            {
                return dt.AddDays(15);
            }
            return dt;
        }

        public static DataReturnModel<object> CreateWithFileExcel(IFormFile file, string name, string accesser)
        {
            var data = new DataReturnModel<object>();
            List<string> err = new List<string>();
            List<string> exist = new List<string>();
            using (Stream stream = file.OpenReadStream())
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = true,
                        ConfigureDataTable = (data) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    DataTableCollection table = result.Tables;
                    DataTable dt = table[name];
                    if (dt == null)
                        dt = table[table.Count - 1];
                    if (dt != null)
                    {

                        var rowCount = dt.Rows.Count;
                        int success = 0;
                        for (int row = 0; row < rowCount; row++)
                        {
                            string customer = dt.Rows[row][1].ToString();
                            if (customer.IndexOf("-") > 0) continue;

                            string code = dt.Rows[row][3].ToString();
                            if (string.IsNullOrEmpty(code)) continue;
                            if (BusinessBase.Exist<tbl_Package>(x => x.PackageCode == code))
                            {
                                exist.Add(code);
                                continue;
                            }

                            int type = Converted.ToInt(dt.Rows[row][2].ToString());
                            string note = dt.Rows[row][4].ToString();
                            var date = Converted.ToDate(dt.Rows[row][0].ToString());
                            var p = new tbl_Package();
                            var user = AccountBusiness.GetByUserName(customer);
                            if (user != null)
                            {
                                p.Username = user.Username;
                                p.UID = user.ID;
                                p.FullName = user.FullName;
                            }

                            if (type == 3)
                                p.MovingMethod = 1;
                            else if (type == 5)
                                p.MovingMethod = 2;

                            p.PackageCode = code;
                            p.Note = note;
                            p.Status = 3;
                            p.OrderDate = date;
                            p.ExportedCNWH = date;
                            p.CreatedDate = DateTime.Now;
                            p.CreatedBy = accesser;
                            if (!BusinessBase.Add(p))
                            {
                                err.Add(code);
                            }
                            else
                                success++;
                        }

                        if (success > 0)
                        {
                            string sSuccess = string.Format("Đã thêm thành công {0} trong tổng số {1}", success, rowCount);
                            data.IsError = false;
                            data.Data = new { success = sSuccess, exist = string.Join("; ", exist), error = string.Join("; ", err) };
                            return data;
                        }
                        else
                        {
                            data.IsError = true;
                            data.Message = "Thêm mới không thành công !";
                            return data;
                        }
                    }
                    data.IsError = true;
                    data.Message = "Không đọc được thông tin file";
                    return data;
                }
            }
        }

        public static DataReturnModel<string> ExportChinaWareHouse(IFormFile file, string name, string accesser)
        {
            var data = new DataReturnModel<string>();
            List<tempExportChina> lstTemp = new List<tempExportChina>();
            using (Stream stream = file.OpenReadStream())
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = true,
                        ConfigureDataTable = (data) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    DataTableCollection table = result.Tables;
                    DataTable dt = table[name];
                    if (dt == null)
                        dt = table[table.Count - 1];
                    if (dt != null)
                    {
                        var rows = dt.Rows.Count;
                        var colums = 2;
                        for (int col = 0; col < colums; col++)
                        {
                            int date = 1;
                            if (col == 0)
                                date = 3;
                            else
                                date = 5;
                            for (int row = 0; row < rows; row++)
                            {
                                lstTemp.Add(new tempExportChina
                                {
                                    Column = col,
                                    Row = row,
                                    MovingMethod = col + 1,
                                    Date = date,
                                    Value = dt.Rows[row][col].ToString()
                                });
                            }
                        }
                    }
                }
            }
            if (lstTemp.Count > 0)
            {
                var codes = lstTemp.Where(x => !string.IsNullOrEmpty(x.Value)).Select(x => x.Value).ToList();
                if (codes.Count > 0)
                {
                    using (var db = new nhanshiphangContext())
                    {
                        var lstPackage = db.tbl_Packages.Where(x => codes.Contains(x.PackageCode) && x.Exported != true).ToList();
                        if (lstPackage.Count > 0)
                        {
                            int i = 0;
                            foreach (var item in lstPackage)
                            {
                                var temp = lstTemp.FirstOrDefault(x => x.Value == item.PackageCode);
                                var dt = DateTime.Now;
                                item.Status = 3;
                                item.MovingMethod = temp.MovingMethod;
                                item.ExportedCNWH = dt;
                                item.DateExpectation = dt.AddDays(temp.MovingMethod);
                                item.Exported = true;
                                item.ModifiedBy = accesser;
                                item.ModifiedDate = DateTime.Now;
                                db.Update(item);

                                temp.Mapped = true;
                                lstTemp[i] = temp;
                                i++;
                            }
                        }
                        var s = db.SaveChanges() > 0;
                        if (s)
                        {
                            string fileName = PJUtils.MarkupValueExcel(file, name, lstTemp.Where(x => x.Mapped).ToList());
                            data.IsError = false;
                            data.Message = "Cập nhật thành công, bạn có muốn tải file đối chiếu?";
                            data.Data = fileName;
                            return data;
                        }
                    }
                }
            }
            else
            {
                data.IsError = true;
                data.Message = "Mã vận đơn đã được cập nhật trước đó !";
                return data;
            }
            data.IsError = true;
            data.Message = "Không đọc được thông tin file";
            return data;
        }
        #endregion
    }
}
