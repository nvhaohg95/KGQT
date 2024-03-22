using ExcelDataReader;
using Fasterflect;
using KGQT.Base;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Data;
using System.Drawing;

namespace KGQT.Business
{
    public static class PackagesBusiness
    {
        #region Select
        public static tbl_Package GetOne(int id)
        {
            using (var db = new nhanshiphangContext())
            {
                var qry = db.tbl_Packages.Where(x => x.ID == id);
                return qry.FirstOrDefault();
            }
        }

        public static List<tbl_Package> GetByTransId(string transId)
        {
            using (var db = new nhanshiphangContext())
            {
                var qry = db.tbl_Packages.Where(x => x.TransID == transId);
                return qry.ToList();
            }
        }
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

        public static List<tbl_Package> GetInStock(string code)
        {
            using (var db = new nhanshiphangContext())
            {
                var qry = db.tbl_Packages.Where(x => x.PackageCode.Contains(code));
                return qry.ToList();
            }
        }

        public static List<tbl_Package> GetByBigPackage(int id)
        {
            var list = BusinessBase.GetList<tbl_Package>(x => x.BigPackage == id).OrderByDescending(x => x.OrderDate).ToList();
            return list;
        }

        public static bool CheckExist(string package)
        {
            using (var db = new nhanshiphangContext())
                return db.tbl_Packages.Any(x => x.PackageCode == package);
        }

        public static object[] GetPage(int status, string ID, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10, string userName = "")
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
                    qry = qry.Where(x => x.CreatedDate <= toDate.Value.AddDays(1).AddTicks(-1));
                if (!string.IsNullOrEmpty(userName))
                    qry = qry.Where(x => x.Username == userName);
                count = qry.Count();
                if (count > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)count / pageSize));
                    qry = qry.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize);

                    lstData = qry.GroupJoin(db.tbl_Accounts, x => x.UID, y => y.ID, (x, y) => new { pack = x, acc = y }).SelectMany(x => x.acc.DefaultIfEmpty(), (p, a) => new tbl_Package
                    {
                        ID = p.pack.ID,
                        PackageCode = p.pack.PackageCode,
                        Status = p.pack.Status,
                        MovingMethod = p.pack.MovingMethod,
                        WeightReal = p.pack.WeightReal,
                        Weight = p.pack.Weight,
                        WeightExchange = p.pack.WeightExchange,
                        Width = p.pack.Width,
                        Height = p.pack.Height,
                        Length = p.pack.Length,
                        Username = p.pack.Username,
                        Phone = a != null && !string.IsNullOrEmpty(a.Phone) ? a.Phone : "",
                        OrderDate = p.pack.OrderDate,
                        ReceivedDate = p.pack.ReceivedDate,
                        ImportedSGWH = p.pack.ImportedSGWH,
                        DateExpectation = p.pack.DateExpectation,
                        ExportedCNWH = p.pack.ExportedCNWH,
                        IsWoodPackage = p.pack.IsWoodPackage,
                        IsAirPackage = p.pack.IsAirPackage,
                        IsInsurance = p.pack.IsInsurance,
                        IsBrand = p.pack.IsBrand,
                        SearchBaiduTimes = p.pack.SearchBaiduTimes,
                        WareHouse = p.pack.WareHouse,
                        CreatedDate = p.pack.CreatedDate
                    }).ToList();

                }
                return new object[] { lstData, count, totalPage };
            }
        }

        public static DataReturnModel<tmpChinaOrderStatus> GetStatusOrder(string code, string uslogin)
        {
            DataReturnModel<tmpChinaOrderStatus> data = new DataReturnModel<tmpChinaOrderStatus>();
            data.Key = code;
            var oPack = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode == code && x.Status < 2);
            if (oPack != null)
            {
                var user = BusinessBase.GetOne<tbl_Account>(x => x.ID == oPack.UID);

                if (oPack.SearchBaiduTimes == null)
                {
                    oPack.SearchBaiduTimes = 0;
                    if (user == null || string.IsNullOrEmpty(user.Wallet) || Converted.ToDouble(user.Wallet) < 500)
                    {
                        data.IsError = true;
                        data.Message = "Tài khoản khách không đủ tiền";
                        return data;
                    }
                    var wallet = Converted.StringCeiling(Converted.ToDouble(user.Wallet) - 500);
                    AccountBusiness.UpdateWallet(user.ID, wallet);

                    #region Logs
                    HistoryPayWallet.Insert(user.ID, user.Username, oPack.ID, "Thanh toán tiền gọi api kiểm tra Mã Vận Đơn " + oPack.PackageCode, "500", 1, 1, wallet, uslogin);
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
                        if (!data.IsError && data.Data.data != null && data.Data.data.Count > 0)
                        {
                            var exist = data.Data.data.FirstOrDefault(x => x.context.Contains("签收") || x.context.Contains("退回"));
                            oPack.Status = 2;
                            oPack.ExportedCNWH = Converted.ToDate(exist.time);
                            if (BusinessBase.Update(oPack))
                            {
                                var admin = BusinessBase.GetOne<tbl_Account>(x => x.Username == uslogin);
                                BusinessBase.TrackLog(admin.ID, oPack.ID, "{0} cập nhật trạng thái kiện - API kiểm tra hàng TQ", 0, admin.Username);
                            }
                        }
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

        public static DataReturnModel<List<tbl_Package>> GetListExport(int status, DateTime? fromDate, DateTime? toDate)
        {
            using (var db = new nhanshiphangContext())
            {
                DataReturnModel<List<tbl_Package>> data = new DataReturnModel<List<tbl_Package>>();
                var query = db.tbl_Packages.Where(x => x.ExportedCNWH != null);
                if (status > 0)
                    query = query.Where(x => x.Status == status);
                if (fromDate != null && fromDate != DateTime.MinValue)
                    query = query.Where(x => x.ExportedCNWH >= fromDate);

                if (toDate != null && toDate != DateTime.MinValue)
                {
                    toDate = toDate.Value.AddDays(1).AddTicks(-1);
                    query = query.Where(x => x.ExportedCNWH <= toDate);
                }
                data.IsError = false;
                data.Data = query.ToList();
                return data;
            }
        }

        #endregion

        #region CRUD
        public static bool Add(tbl_Package model, string userLogin)
        {
            var user = AccountBusiness.GetInfo(-1, model.Username);
            model.Status = 1;
            model.PackageCode = model.PackageCode.Trim().Replace("\'", "").Replace(" ", "");

            if (user != null)
            {
                model.UID = user.ID;
                model.Username = user.Username;
                model.FullName = user.FullName;
                model.Phone = user.Phone;
                model.Email = user.Email;
                model.Address = user.Address;
            }

            model.OrderDate = DateTime.Now;
            model.CreatedDate = DateTime.Now;
            model.CreatedBy = userLogin;
            model.Weight = "0";
            model.WeightExchange = "0";
            model.Height = "0";
            model.Width = "0";
            model.Length = "0";
            model.WeightReal = "0";
            if (model.IsInsurance.HasValue && model.IsInsurance == true)
            {
                model.IsInsurancePrice = Converted.StringCeiling(Converted.ToDouble(model.DeclarePrice) * 0.05);
            }

            var s = BusinessBase.Add(model);
            return s;
        }

        public static DataReturnModel<bool> CustomerAdd(tbl_Package form, string userLogin)
        {
            var data = new DataReturnModel<bool>();
            var user = BusinessBase.GetOne<tbl_Account>(x => x.Username == userLogin);
            form.PackageCode = form.PackageCode.Trim().Replace("\'", "").Replace(" ", "");

            var exist = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode == form.PackageCode);

            if (exist != null)
            {
                if (exist.Username == userLogin || string.IsNullOrEmpty(exist.Username))
                {
                    exist.UID = user.ID;
                    exist.Username = user.Username;
                    exist.FullName = user.FullName;
                    exist.Address = user.Address;
                    exist.Phone = user.Phone;
                    exist.Email = user.Email;
                }
                else if (!string.IsNullOrEmpty(exist.Username) && exist.Username != userLogin)
                {
                    data.IsError = true;
                    data.Message = "Mã vận đơn đã có trong hệ thống, vui lòng liên hệ với nhân viên để đối chiếu thông tin!";
                    return data;
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
                    form.IsInsurancePrice = Converted.StringCeiling(Converted.ToDouble(form.DeclarePrice) * 0.05);
                }

                exist.ModifiedBy = user.Username;
                exist.ModifiedDate = DateTime.Now;
                var update = BusinessBase.Update(exist);
                if (update)
                    BusinessBase.TrackLog(user.ID, form.ID, "{0} đã cập nhật thông tin kiện", 0, user.Username);
                data.IsError = false;
                data.Message = "Tạo Mã vận đơn thành công!";
                return data;
            }

            form.Status = 1;
            form.UID = user.ID;
            form.Username = user.Username;
            form.FullName = user.FullName;
            form.Phone = user.Phone;
            form.Email = user.Email;
            form.Address = user.Address;
            form.OrderDate = DateTime.Now;
            form.CreatedDate = DateTime.Now;
            form.CreatedBy = user.Username;
            form.Weight = "0";
            form.WeightExchange = "0";
            form.Height = "0";
            form.Width = "0";
            form.Length = "0";
            form.WeightReal = "0";
            if (form.IsInsurance.HasValue && form.IsInsurance == true)
            {
                form.IsInsurancePrice = Converted.StringCeiling(Converted.ToDouble(form.DeclarePrice) * 0.05);
            }

            var s = BusinessBase.Add(form);
            if (s)
            {
                data.IsError = false;
                data.Message = "Đã thêm mã vận đơn";
                BusinessBase.TrackLog(user.ID, form.ID, "{0} đã tạo kiện", 0, user.Username);
                return data;
            }
            data.IsError = true;
            data.Message = "Thêm mới không thành công";
            return data;
        }

        public static bool Update(tbl_Package form, string userLogin)
        {
            try
            {
                var p = BusinessBase.GetOne<tbl_Package>(x => x.ID == form.ID);
                if (p == null) return false;
                int oldStt = p.Status;
                if (!string.IsNullOrEmpty(form.Username) && form.Username.ToLower() != p.Username.ToLower())
                {
                    var user = BusinessBase.GetOne<tbl_Account>(x => x.Username.ToLower() == form.Username.ToLower());
                    if (user != null)
                    {
                        p.Username = user.Username;
                        p.UID = user.ID;
                        p.FullName = user.FullName;
                    }
                }
                p.BigPackage = form.BigPackage;
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
                p.SurCharge = form.SurCharge;
                p.ModifiedBy = userLogin;
                p.ModifiedDate = DateTime.Now;
                if (oldStt != p.Status)
                {
                    if (p.Status == 3)
                    {
                        p.ExportedCNWH = DateTime.Now;
                        var d = PJUtils.GetDeliveryDate(DateTime.Now, p.MovingMethod);
                        p.DateExpectation = "Dự kiến " + d.ToString("dd/MM/yyyy");

                        p.Exported = true;
                    }
                }
                var update = BusinessBase.Update(p);
                if (update)
                {
                    var admin = BusinessBase.GetOne<tbl_Account>(x => x.Username == userLogin);
                    BusinessBase.TrackLog(admin.ID, p.ID, "{0} đã chỉnh sửa kiện", 0, admin.Username);
                }
                return update;
            }
            catch (Exception ex)
            {
                Log.Error("Lỗi cập nhật package", JsonConvert.SerializeObject(ex));
                return false;
            }
        }
        public static bool UpdateTrans(int id, string transId)
        {
            using (var db = new nhanshiphangContext())
            {
                var pack = db.tbl_Packages.FirstOrDefault(x => x.ID == id);
                if (pack == null) return false;

                pack.TransID = transId;
                db.Update(pack);
                return db.SaveChanges() > 0;
            }
        }
        public static bool InStockHCMWareHouse(tmpInStock data, string accessor)
        {
            var pack = BusinessBase.GetOne<tbl_Package>(x => x.ID == data.Id);
            if (pack == null) return false;
            var admin = BusinessBase.GetOne<tbl_Account>(x => x.Username == accessor);
            if (!string.IsNullOrEmpty(data.Username) && pack.Username != data.Username)
            {
                var user = BusinessBase.GetOne<tbl_Account>(x => x.Username.ToLower() == data.Username.ToLower());
                pack.Username = data.Username;
                pack.UID = user?.ID;
                pack.FullName = user?.FullName;
            }

            if (data.MovingMethod > 0 && pack.MovingMethod != data.MovingMethod)
                pack.MovingMethod = data.MovingMethod;

            if (data.Height > 0 || data.Width > 0 || data.Length > 0)
                data.WeightExchange = GetExchangeWeight(pack.MovingMethod, data);

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

            var fee = lstFee.FirstOrDefault(x => x.WeightFrom <= data.Weight && data.Weight <= x.WeightTo);

            if (lstFee == null || lstFee.Count == 0) return false;

            pack.Weight = data.Weight.ToString();
            pack.WeightExchange = data.WeightExchange.ToString();
            pack.Height = data.Height.ToString();
            pack.Width = data.Width.ToString();
            pack.Length = data.Length.ToString();
            pack.WoodPackagePrice = data.WoodPrice.ToString();
            pack.AirPackagePrice = data.AirPrice.ToString();
            pack.SurCharge = data.SurCharge.ToString();
            pack.Status = 4;
            pack.ModifiedBy = accessor;
            pack.ModifiedDate = DateTime.Now;
            pack.ImportedSGWH = DateTime.Now;

            if (pack.MovingMethod < 3)
            {
                var maxWeight = data.Weight * (25 / (float)100);
                if (data.WeightExchange > (data.Weight + maxWeight) || data.WeightExchange > (data.Weight + 1))
                    pack.WeightReal = data.WeightExchange.ToString();
                else
                    pack.WeightReal = data.Weight.ToString();
            }
            else
            {
                var maxWeight = data.Weight * (50 / (float)100);
                if (data.WeightExchange >= (data.Weight + maxWeight) || data.WeightExchange >= (data.Weight + 2))
                    pack.WeightReal = pack.WeightExchange;
                else
                    pack.WeightReal = pack.Weight;
            }

            if (Converted.ToDouble(pack.Weight) <= minPackage)
                pack.Weight = minPackage.ToString();

            if (Converted.ToDouble(pack.WeightReal) <= minPackage)
                pack.WeightReal = minPackage.ToString();

            if (pack.IsInsurance == true && data.DeclarePrice > 0)
            {
                pack.IsInsurancePrice = Converted.StringCeiling(Converted.ToDouble(data.DeclarePrice) * 0.05);
                pack.Declaration = data.Link;
                pack.DeclarePrice = data.DeclarePrice.ToString();
            }

            var p = BusinessBase.Update(pack);
            if (p)
            {
                //Ktra xem khach da co don chua.
                DateTime cnExportDateFrom = DateTime.Now;
                if (pack.ExportedCNWH != null)
                    cnExportDateFrom = pack.ExportedCNWH.Value.Date;
                else
                    pack.ExportedCNWH = cnExportDateFrom;
                DateTime cnExportDateEnd = pack.ExportedCNWH.Value.AddDays(1).AddTicks(-1);
                DateTime startDate = DateTime.Now.Date; //One day 
                DateTime endDate = startDate.AddDays(1).AddTicks(-1);
                var check = ShippingOrder.CheckOrderInStock(pack.Username, pack.MovingMethod, startDate, endDate, cnExportDateFrom, cnExportDateEnd);
                if (check != null)
                {
                    //Cap nhat lai transid cho p
                    pack.TransID = check.RecID;
                    pack.ModifiedBy = accessor;
                    pack.ModifiedDate = DateTime.Now;
                    UpdateTrans(pack.ID, check.RecID);
                    var lstPack = GetByTransId(check.RecID);

                    double weight = 0;
                    double totalWeightPrice = 0;
                    double priceBrand = 0;
                    double weightBrand = 0;
                    foreach (var item in lstPack)
                    {
                        if (item.IsBrand == true)
                            weightBrand += Converted.ToDouble(item.WeightReal);
                        else
                            weight += Converted.ToDouble(item.WeightReal);
                    }

                    if (weightBrand > 0 && weightBrand < minPackage)
                        weightBrand = minPackage;
                    if (weightBrand > 0)
                    {
                        var feeBrand = lstFee.FirstOrDefault(x => x.WeightFrom <= weightBrand && weightBrand <= x.WeightTo);
                        priceBrand = Converted.DoubleCeiling((Converted.ToDouble(feeBrand.PriceBrand) + Converted.ToDouble(feeBrand.Amount)) * weightBrand);
                    }

                    double totalWeight = weight + weightBrand;
                    var finalfee = lstFee.FirstOrDefault(x => x.WeightFrom <= weight && weight <= x.WeightTo);
                    if (finalfee != null)
                    {
                        if ((totalWeight + weightBrand) < minOrder)
                        {
                            totalWeightPrice = Converted.DoubleCeiling((minOrder - weightBrand) * Converted.ToDouble(finalfee.Amount));
                            weight = minOrder;
                        }
                        else
                        {
                            totalWeightPrice = Converted.DoubleCeiling(weight * Converted.ToDouble(finalfee.Amount));
                        }
                    }

                    double totalWoodPrice = lstPack.Where(x => x.WoodPackagePrice != null).Sum(x => Converted.ToDouble(x.WoodPackagePrice));
                    double totalAirPrice = lstPack.Where(x => x.AirPackagePrice != null).Sum(x => Converted.ToDouble(x.AirPackagePrice));
                    double totalInsurPrice = lstPack.Where(x => x.IsInsurancePrice != null).Sum(x => Converted.ToDouble(x.IsInsurancePrice));
                    double totalCharge = lstPack.Sum(x => Converted.ToDouble(x.SurCharge));

                    var totalPrice = totalWeightPrice + totalWoodPrice + totalAirPrice + totalInsurPrice + totalCharge + priceBrand;
                    totalWeightPrice = totalWeightPrice + priceBrand;
                    check.Weight = weight.ToString();
                    check.WeightPrice = Converted.StringCeiling(totalWeightPrice);
                    check.WoodPackagePrice = totalWoodPrice.ToString();
                    check.AirPackagePrice = totalAirPrice.ToString();
                    check.InsurancePrice = totalInsurPrice.ToString();
                    check.TotalPrice = totalPrice.ToString();
                    check.ModifiedBy = accessor;
                    check.ModifiedDate = DateTime.Now;
                    var oUpdate = BusinessBase.Update(check);
                }
                else
                {
                    var weightRound = Converted.ToDouble(pack.WeightReal);
                    double feeWeight = 0;

                    if (weightRound < minOrder)
                        weightRound = minOrder;

                    feeWeight = Converted.DoubleCeiling(weightRound * Converted.ToDouble(fee.Amount));

                    if (pack.IsBrand == true)
                    {
                        feeWeight = Converted.DoubleCeiling((Converted.ToDouble(fee.PriceBrand) + Converted.ToDouble(fee.Amount)) * weightRound);
                    }

                    var ship = new tbl_ShippingOrder();
                    ship.ShippingOrderCode = pack.PackageCode;
                    ship.RecID = Guid.NewGuid().ToString("N");
                    ship.Username = pack.Username;
                    ship.FirstName = pack.FullName;
                    ship.LastName = pack.FullName;
                    ship.Email = pack.Email;
                    ship.Address = pack.Address;
                    ship.Phone = pack.Phone;
                    ship.ShippingMethod = pack.MovingMethod;
                    ship.ShippingMethodName = PJUtils.ShippingMethodName(pack.MovingMethod);
                    ship.Weight = weightRound.ToString();
                    ship.WeightPrice = feeWeight.ToString();
                    ship.IsAirPackage = pack.IsAirPackage;
                    ship.AirPackagePrice = pack.AirPackagePrice;
                    ship.IsWoodPackage = pack.IsWoodPackage;
                    ship.WoodPackagePrice = pack.WoodPackagePrice;
                    ship.IsInsurance = pack.IsInsurance;
                    ship.InsurancePrice = pack.IsInsurancePrice;
                    ship.SurCharge = pack.SurCharge;
                    ship.TotalPrice = Converted.Double2String(feeWeight
                        + Converted.ToDouble(pack.AirPackagePrice)
                        + Converted.ToDouble(pack.WoodPackagePrice)
                        + Converted.ToDouble(pack.IsInsurancePrice)
                        + Converted.ToDouble(pack.SurCharge));
                    ship.Status = 1;
                    ship.ChinaExportDate = pack.ExportedCNWH;
                    ship.CreatedDate = DateTime.Now;
                    ship.CreatedBy = accessor;
                    var oAdd = BusinessBase.Add(ship);
                    if (oAdd)
                    {
                        var oPack = BusinessBase.GetOne<tbl_Package>(x => x.ID == pack.ID);
                        if (oPack != null)
                        {
                            oPack.TransID = ship.RecID;
                            oPack.ModifiedBy = accessor;
                            oPack.ModifiedDate = DateTime.Now;
                            BusinessBase.Update(oPack);
                        }
                        NotificationBusiness.Insert(admin.ID, admin.Username, pack.UID, pack.Username, ship.ID, ship.ShippingOrderCode, "Đơn hàng " + ship.ShippingOrderCode + " đã nhập kho HCM", 1, "/ShippingOrder/Details/" + ship.ID, accessor);
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
            var lstError = new List<tempExport>();
            List<string> err = new List<string>();
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
                    System.Data.DataTable dt = table[name];
                    if (dt == null)
                        dt = table[table.Count - 1];
                    if (dt != null)
                    {

                        var rowCount = dt.Rows.Count;

                        var exit = BusinessBase.Exist<tbl_BigPackage>(x => x.FileName == file.FileName && x.DataCount == rowCount);
                        if (exit)
                        {
                            data.IsError = true;
                            data.Message = "File này đã được upload";
                            stream.Dispose();
                            return data;
                        }
                        int success = 0;
                        var big = new tbl_BigPackage();
                        big.BigPackageCode = file.FileName + "_" + DateTime.Now.ToString("ddMMyyyy");
                        big.DataCount = rowCount;
                        big.FileName = file.FileName;
                        big.CreatedDate = DateTime.Now;
                        big.CreatedBy = accesser;
                        if (BusinessBase.Add(big))
                        {
                            for (int row = 0; row < rowCount; row++)
                            {
                                try
                                {
                                    string customer = dt.Rows[row][1].ToString();
                                    if (customer.IndexOf("-") > 0)
                                    {
                                        lstError.Add(new tempExport { Row = row, Column = 3 });
                                        continue;
                                    }
                                    string code = dt.Rows[row][3].ToString();
                                    if (string.IsNullOrEmpty(code))
                                    {
                                        lstError.Add(new tempExport { Row = row, Column = 3 });
                                        continue;
                                    }

                                    int type = Converted.ToInt(dt.Rows[row][2].ToString());
                                    string note = dt.Rows[row][4].ToString();
                                    var date = DateTime.Parse(dt.Rows[row][0].ToString());
                                    var dateExp = DateTime.Now;
                                    if (date != null || date != DateTime.MinValue && type != null || type > 0)
                                    {
                                        dateExp = date.AddDays(type);
                                    }
                                    var d = PJUtils.GetDeliveryDate(dateExp);
                                    var oExist = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode == code);
                                    if (oExist != null)
                                    {
                                        if (oExist.Status <= 3)
                                        {
                                            oExist.Status = 3;
                                            oExist.Note = note;
                                            oExist.Status = 3;
                                            oExist.OrderDate = date;
                                            oExist.ExportedCNWH = date;
                                            oExist.DateExpectation = "Dự kiến " + d.ToString("dd/MM/yyyy");
                                            oExist.ModifiedDate = DateTime.Now;
                                            oExist.ModifiedBy = accesser;
                                            if (oExist.BigPackage == null || oExist.BigPackage < 0)
                                                oExist.BigPackage = big.ID;
                                            if (BusinessBase.Update(oExist)) success++;
                                            else
                                            {
                                                lstError.Add(new tempExport { Row = row, Column = 3 });
                                                err.Add(code);
                                            }
                                        }
                                        continue;
                                    }


                                    var p = new tbl_Package();
                                    var user = BusinessBase.GetOne<tbl_Account>(x => x.Username.ToLower() == customer.ToLower());
                                    if (user != null)
                                    {
                                        p.Username = user.Username;
                                        p.UID = user.ID;
                                        p.FullName = user.FullName;
                                    }

                                    if (type == 3)
                                        p.MovingMethod = 1;
                                    if (type == 5)
                                        p.MovingMethod = 2;
                                    if (type == 8)
                                        p.MovingMethod = 3;
                                    if (type == 15)
                                        p.MovingMethod = 4;
                                    if (type == 20)
                                        p.MovingMethod = 5;
                                    p.PackageCode = code;
                                    p.Note = note;
                                    p.Status = 3;
                                    p.Weight = "0";
                                    p.WeightExchange = "0";
                                    p.Height = "0";
                                    p.Width = "0";
                                    p.Length = "0";
                                    p.WeightReal = "0";
                                    p.OrderDate = date;
                                    p.ExportedCNWH = date;
                                    p.DateExpectation = "Dự kiến " + d.ToString("dd/MM/yyyy");
                                    p.CreatedDate = DateTime.Now;
                                    p.CreatedBy = accesser;
                                    p.BigPackage = big.ID;
                                    if (!BusinessBase.Add(p))
                                    {
                                        err.Add(code);
                                        lstError.Add(new tempExport { Row = row, Column = 3 });
                                    }
                                    else
                                        success++;
                                }
                                catch (Exception ex)
                                {
                                    Log.Error("Import Excel", JsonConvert.SerializeObject(ex.Message));
                                    lstError.Add(new tempExport { Row = row, Column = 3 });
                                }
                            }
                            if (success > 0)
                            {
                                string sSuccess = string.Format("Đã thêm thành công {0} trong tổng số {1}", success, rowCount - 1);
                                data.IsError = false;
                                string error = string.Join("; ", err);
                                data.Data = new { success = sSuccess, error = string.Join("; ", err) };
                                MarkupValueExcel(stream, file.FileName, name, lstError);
                                return data;
                            }
                            else
                            {
                                BusinessBase.Remove(big);
                                data.IsError = true;
                                data.Message = "Thêm mới không thành công !";
                                stream.Dispose();
                                return data;
                            }
                        }
                    }
                    data.IsError = true;
                    data.Message = "Không đọc được thông tin file";
                    stream.Dispose();
                    return data;
                }
            }
        }

        public static string MarkupValueExcel(Stream file, string fileName, string name, List<tempExport> data)
        {
            if (file == null) return "";
            try
            {
                using (ExcelPackage pack = new ExcelPackage(file))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelWorksheet worksheet = null;
                    if (!string.IsNullOrEmpty(name))
                        worksheet = (ExcelWorksheet)pack.Workbook.Worksheets.GetIndexer(new object[] { name });
                    else
                        worksheet = pack.Workbook.Worksheets.LastOrDefault();

                    if (worksheet == null)
                        return "";
                    else
                    {
                        #region Read value
                        foreach (var item in data)
                        {
                            worksheet.Cells[item.Row + 2, item.Column + 1].Style.Fill.SetBackground(Color.Red);
                        }
                        #endregion

                        #region save file was edit
                        try
                        {
                            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                            pathRoot = Path.Combine(pathRoot, "ExportExcel", DateTime.Now.ToString("ddMMyyyy"));
                            if (!Directory.Exists(pathRoot))
                                Directory.CreateDirectory(pathRoot);

                            string sheet = worksheet.Name;
                            pathRoot = pathRoot + "/" + fileName;

                            pack.SaveAs(pathRoot);
                            byte[] fileBytes = System.IO.File.ReadAllBytes(pathRoot);
                            return pathRoot;
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Export file excel", JsonConvert.SerializeObject(ex));
                            return "";
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static DataReturnModel<bool> Cancel(int id, string username)
        {
            var data = new DataReturnModel<bool>();
            var pack = BusinessBase.GetOne<tbl_Package>(x => x.ID == id && x.Status <= 2);
            if (pack != null)
            {
                pack.Status = 9;
                pack.ModifiedBy = username;
                pack.ModifiedDate = DateTime.Now;
                if (BusinessBase.Update(pack))
                {
                    data.IsError = false;
                    data.Message = "Hủy đơn thành công!";
                    return data;
                }
            }
            data.IsError = true;
            data.Message = "Hủy không thành công, vui lòng liên hệ với nhân viên để được giải quyết !";
            return data;
        }

        public static double GetExchangeWeight(int type, tmpInStock data)
        {
            double? w = 0;
            var total = (data.Length * data.Height * data.Width) / 6000;
            switch (type)
            {
                case 1:
                case 2:
                    w = ((total + data.Weight) / 2);
                    break;
                case 3:
                case 4:
                    w = total;
                    break;
                case 5:
                    w = (data.Length * data.Height * data.Width) / 5000;
                    break;
            }
            return Converted.ToDouble(w);
        }
    }
    #endregion
}
