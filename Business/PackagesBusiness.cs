using ExcelDataReader;
using Fasterflect;
using KGQT.Base;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Newtonsoft.Json;
using OfficeOpenXml;
using Serilog;
using System.Data;
using System.Drawing;
using ILogger = Serilog.ILogger;

namespace KGQT.Business
{
    public static class PackagesBusiness
    {
        private static readonly ILogger _log = Log.ForContext(typeof(PackagesBusiness));

        #region Select
        public static tbl_Package GetOne(int id)
        {
            using (var db = new nhanshiphangContext())
            {
                var qry = db.tbl_Packages.Where(x => x.ID == id);
                return qry.FirstOrDefault();
            }
        }

        public static tbl_Package GetOne(string code)
        {
            using (var db = new nhanshiphangContext())
            {
                var qry = db.tbl_Packages.Where(x => x.PackageCode == code);
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
                var qry = db.tbl_Packages.Where(x => x.PackageCode.Contains(code) && x.Status != 9);
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
            var oPack = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode == code);
            if (oPack != null)
            {
                var user = BusinessBase.GetOne<tbl_Account>(x => x.Username == oPack.Username);
                if (user != null)
                {
                    if (user.AvailableSearch == 0)
                    {
                        if (string.IsNullOrEmpty(user.Wallet) || Converted.ToDouble(user.Wallet) < 500)
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
                    else
                        AccountBusiness.UpdateSearch(user.ID, user.AvailableSearch - 1);
                }
                using (HttpClient client = new HttpClient())
                {
                    string url = string.Format(Config.Settings.ApiUrl, Config.Settings.ApiKey, code);
                    var response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string sData = response.Content.ReadAsStringAsync().Result;
                        data.Data = JsonConvert.DeserializeObject<tmpChinaOrderStatus>(sData);
                        data.IsError = !data.Data.success;
                        if (!data.IsError && data.Data.data != null && data.Data.data.Count > 0)
                        {
                            var exist = data.Data.data.FirstOrDefault(x => x.context.Contains("签收") || x.context.Contains("退回"));
                            if (exist != null)
                            {
                                if (oPack.Status < 2)
                                    oPack.Status = 2;
                                oPack.ExportedCNWH = Converted.ToDate(exist.time);
                                if (BusinessBase.Update(oPack))
                                {
                                    var admin = BusinessBase.GetOne<tbl_Account>(x => x.Username == uslogin);
                                    BusinessBase.TrackLog(admin.ID, oPack.ID, "{0} cập nhật trạng thái kiện - API kiểm tra hàng TQ", 0, admin.Username);
                                }
                            }
                            for (int i = 0; i < data.Data.data.Count; i++)
                            {
                                string stran = PJUtils.RemoveHTMLTags(PJUtils.TranslateTextNew(data.Data.data[i].context, "zh", "vi"));
                                string properties_trans = stran.Replace("[", "").Replace("]", "").Replace("\"", "");
                                string[] ass = properties_trans.Split(',');
                                data.Data.data[i].context = ass[0];
                            }
                        }
                        else
                        {
                            string reason = PJUtils.RemoveHTMLTags(PJUtils.TranslateTextNew(data.Data.reason, "zh", "vi"));
                            string properties_trans = reason.Replace("[", "").Replace("]", "").Replace("\"", "");
                            string[] ass = properties_trans.Split(',');
                            data.Data.reason = ass[0];
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
                data.IsError = true;
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
        public static DataReturnModel<bool> Add(tbl_Package model, string userLogin)
        {
            var dt = new DataReturnModel<bool>();
            var user = AccountBusiness.GetInfo(-1, model.Username);
            model.Status = 1;
            model.PackageCode = model.PackageCode.Trim().Replace("\'", "").Replace(" ", "");
            model.PackageCode = model.PackageCode.Replace(",", ";");
            var lst1 = model.PackageCode.Split(";", StringSplitOptions.RemoveEmptyEntries);
            int fail = 0;
            foreach (var item in lst1)
            {
                var exist = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode == item);
                if (exist != null)
                {
                    dt.IsError = true;
                    dt.Message += $"Mã vận đơn {item} đã tốn tại <hr>";
                    continue;
                }
                var pack = new tbl_Package();
                if (user != null)
                {
                    pack.UID = user.ID;
                    pack.Username = user.Username;
                    pack.FullName = user.FullName;
                    pack.Phone = user.Phone;
                    pack.Email = user.Email;
                    pack.Address = user.Address;
                }
                pack.PackageCode = item;
                pack.WareHouse = model.WareHouse;
                pack.MovingMethod = model.MovingMethod;
                pack.IsAirPackage = model.IsAirPackage;
                pack.IsWoodPackage = model.IsWoodPackage;
                pack.IsInsurance = model.IsInsurance;
                pack.IsBrand = model.IsBrand;
                pack.SurCharge = model.SurCharge;
                pack.Note = model.Note;
                pack.BigPackage = model.BigPackage;
                pack.OrderDate = DateTime.Now;
                pack.CreatedDate = DateTime.Now;
                pack.CreatedBy = userLogin;
                pack.Weight = "0";
                pack.WeightExchange = "0";
                pack.Height = "0";
                pack.Width = "0";
                pack.Length = "0";
                pack.WeightReal = "0";
                pack.SearchBaiduTimes = 0;
                if (model.IsInsurance.HasValue && model.IsInsurance == true)
                {
                    pack.IsInsurancePrice = Converted.StringCeiling(Converted.ToDouble(model.DeclarePrice) * 0.05);
                }

                if (BusinessBase.Add(pack))
                {
                    dt.Message += $"Đã thêm Mã vận đơn {item} </br>";
                }
            }
            if (fail == lst1.Length)
            {
                dt.IsError = true;
                dt.Message = "Thêm mới không thành công";
            }
            else
            {
                dt.IsError = false;
            }
            return dt;
        }

        public static DataReturnModel<bool> CustomerAdd(tbl_Package form, string userLogin)
        {
            var data = new DataReturnModel<bool>();
            var user = BusinessBase.GetOne<tbl_Account>(x => x.Username == userLogin);
            form.PackageCode = form.PackageCode.Trim().Replace("\'", "").Replace(" ", "");
            form.PackageCode = form.PackageCode.Replace(",", ";");
            var lst1 = form.PackageCode.Split(";", StringSplitOptions.RemoveEmptyEntries);
            int fail = 0;
            foreach (var item in lst1)
            {
                bool equalVC = true;
                var exist = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode == item);

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
                        data.Message = $"Mã vận đơn {item} đã có trong hệ thống, vui lòng liên hệ với nhân viên để đối chiếu thông tin! <hr>";
                        fail++;
                        continue;
                    }

                    if (form.MovingMethod > 0 && exist.MovingMethod != form.MovingMethod)
                        equalVC = true;

                    exist.IsAirPackage = form.IsAirPackage;
                    exist.IsWoodPackage = form.IsWoodPackage;
                    exist.IsInsurance = form.IsInsurance;
                    if (form.IsInsurance.HasValue && form.IsInsurance == true)
                    {
                        exist.Declaration = form.Declaration;
                        exist.DeclarePrice = form.DeclarePrice;
                        exist.IsInsurancePrice = Converted.StringCeiling(Converted.ToDouble(form.DeclarePrice) * 0.05);
                    }

                    exist.ModifiedBy = user.Username;
                    exist.ModifiedDate = DateTime.Now;
                    var update = BusinessBase.Update(exist);
                    if (update)
                        BusinessBase.TrackLog(user.ID, form.ID, "{0} đã cập nhật thông tin kiện", 0, user.Username);
                    data.IsError = false;
                    data.Message = "Tạo Mã vận đơn thành công!";
                    if (equalVC)
                        data.Message = $"Mã vận đơn {item} đã được nhân viên khai báo phương thức vận chuyển." +
                            $" Để thay đổi phương thức vận chuyển vui lòng liên hệ nhân viên Nhanshiphang để được giúp đỡ <hr>";
                    continue;
                }
                else
                {
                    var pack = new tbl_Package();
                    pack.PackageCode = item;
                    pack.WareHouse = form.WareHouse;
                    pack.MovingMethod = form.MovingMethod;
                    pack.IsAirPackage = form.IsAirPackage;
                    pack.IsWoodPackage = form.IsWoodPackage;
                    pack.IsInsurance = form.IsInsurance;
                    pack.Status = 1;
                    pack.UID = user.ID;
                    pack.Username = user.Username;
                    pack.FullName = user.FullName;
                    pack.Phone = user.Phone;
                    pack.Email = user.Email;
                    pack.Address = user.Address;
                    pack.OrderDate = DateTime.Now;
                    pack.CreatedDate = DateTime.Now;
                    pack.CreatedBy = user.Username;
                    pack.Weight = "0";
                    pack.WeightExchange = "0";
                    pack.Height = "0";
                    pack.Width = "0";
                    pack.Length = "0";
                    pack.WeightReal = "0";
                    pack.SearchBaiduTimes = 0;
                    pack.Note = form.Note;
                    if (form.IsInsurance.HasValue && form.IsInsurance == true)
                    {
                        pack.IsInsurancePrice = Converted.StringCeiling(Converted.ToDouble(form.DeclarePrice) * 0.05);
                    }

                    var s = BusinessBase.Add(pack);
                    if (s)
                    {
                        data.Message += $"Đã thêm mã vận đơn {item}</br>";
                        BusinessBase.TrackLog(user.ID, form.ID, "{0} đã tạo kiện", 0, user.Username);
                    }
                }
            }
            if (fail == lst1.Length)
            {
                data.IsError = true;
                data.Message = "Thêm mới không thành công";

            }
            else
                data.IsError = false;
            return data;
        }

        public static DataReturnModel<bool> Update(tbl_Package form, string userLogin)
        {
            using (var db = new nhanshiphangContext())
            {
                var dt = new DataReturnModel<bool>();
                try
                {
                    var orin = db.tbl_Packages.FirstOrDefault(x => x.ID == form.ID);
                    var p = db.tbl_Packages.FirstOrDefault(x => x.ID == form.ID);
                    if (p == null)
                    {
                        dt.IsError = true;
                        dt.Message = "Không tìm thấy thông tin kiện";
                        return dt;
                    }

                    int oldStt = p.Status;
                    int oldVc = p.MovingMethod;

                    if (!string.IsNullOrEmpty(p.Username))
                    {
                        var user = db.tbl_Accounts.FirstOrDefault(x => x.Username.ToLower() == form.Username.ToLower());
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
                    p.Declaration = form.Declaration;
                    p.IsWoodPackage = form.IsWoodPackage;

                    if (p.AirPackagePrice != form.AirPackagePrice)
                        p.AirPackagePrice = form.AirPackagePrice;

                    if (p.IsInsurancePrice != form.IsInsurancePrice)
                        p.IsInsurancePrice = form.IsInsurancePrice;

                    if (p.WoodPackagePrice != form.WoodPackagePrice)
                        p.WoodPackagePrice = form.WoodPackagePrice;

                    if (p.DeclarePrice != form.DeclarePrice)
                        p.DeclarePrice = form.DeclarePrice;

                    if (p.IsBrand != form.IsBrand)
                        p.IsBrand = form.IsBrand;

                    if (p.SurCharge != form.SurCharge)
                        p.SurCharge = form.SurCharge;

                    p.WareHouse = form.WareHouse;
                    p.Status = form.Status;
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
                        var admin = db.tbl_Accounts.FirstOrDefault(x => x.Username == userLogin);

                        if (!string.IsNullOrEmpty(p.TransID))
                        {
                            var ship = db.tbl_ShippingOrders.FirstOrDefault(x => x.RecID == p.TransID);
                            int countPackold = db.tbl_Packages.Where(x => x.TransID == ship.RecID).Count();

                            if (ship != null)
                            {
                                if (ship.Username != p.Username)
                                {
                                    var shipnew = ShippingOrder.CheckOrderInStock(p.Username, p.MovingMethod, ship.CreatedDate.Value.Date,
                                    ship.CreatedDate.Value.Date.AddDays(1).AddTicks(-1), p.ExportedCNWH.Value.Date, p.ExportedCNWH.Value.Date.AddDays(1).AddTicks(-1), ship.RecID);

                                    if (shipnew != null)
                                    {
                                        var user = db.tbl_Accounts.FirstOrDefault(x => x.Username.ToLower() == p.Username.ToLower());
                                        var packe = BusinessBase.GetOne<tbl_Package>(x => x.ID == p.ID);
                                        p.TransID = shipnew.RecID;
                                        packe.TransID = shipnew.RecID;
                                        BusinessBase.Update(packe);
                                        BusinessBase.TrackLog(admin.ID, p.ID, "{0} đã chuyển kiện từ đơn " + ship.RecID + " sang đơn " + shipnew.RecID, 0, admin.Username);
                                    }
                                    else
                                    {
                                        p = BusinessBase.GetOne<tbl_Package>(x => x.ID == p.ID);
                                        var s = ShippingOrder.Add(p, userLogin);
                                        if (!s)
                                        {
                                            BusinessBase.Update(orin);
                                            dt.IsError = true;
                                            dt.Message = "Cập nhật không thành công!";
                                            return dt;
                                        }
                                    }
                                }
                                else
                                {
                                    if (oldVc != p.MovingMethod)
                                    {
                                        p = BusinessBase.GetOne<tbl_Package>(x => x.ID == p.ID);
                                        var s = ShippingOrder.Add(p, userLogin);
                                        if (!s)
                                        {
                                            BusinessBase.Update(orin);
                                            dt.IsError = true;
                                            dt.Message = "Cập nhật không thành công!";
                                            return dt;
                                        }
                                    }
                                }
                            }

                            p = BusinessBase.GetOne<tbl_Package>(x => x.ID == p.ID);
                            ShippingOrder.CalculatorAllPrice(p.TransID, userLogin);
                            ShippingOrder.UpdateByPackageChanged(p, userLogin);
                        }

                        BusinessBase.TrackLog(admin.ID, p.ID, "{0} đã chỉnh sửa kiện", 0, admin.Username);
                        dt.IsError = false;
                        dt.Message = "Cập nhật thành công!";
                        return dt;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Lỗi cập nhật package", ex.Message);
                }
                dt.IsError = false;
                dt.Message = "Cập nhật thành công!";
                return dt;
            }
        }

        public static DataReturnModel<bool> InStockHCMWareHouse(tmpInStock data, string accessor)
        {
            using (var db = new nhanshiphangContext())
            {
                var dt = new DataReturnModel<bool>();
                try
                {
                    var pack = db.tbl_Packages.FirstOrDefault(x => x.ID == data.Id);
                    if (pack == null)
                    {
                        dt.IsError = true;
                        dt.Message = "Không tìm thấy thông tin kiện";
                        return dt;
                    };

                    var admin = db.tbl_Accounts.FirstOrDefault(x => x.Username == accessor);
                    tbl_Account user = null;
                    if (!string.IsNullOrEmpty(data.Username))
                    {
                        user = db.tbl_Accounts.FirstOrDefault(x => x.Username.ToLower() == data.Username.ToLower());
                        pack.Username = data.Username;
                        pack.UID = user?.ID;
                        pack.FullName = user?.FullName;
                    }

                    if (data.MovingMethod > 0 && pack.MovingMethod != data.MovingMethod)
                        pack.MovingMethod = data.MovingMethod;

                    if (data.Height > 0 || data.Width > 0 || data.Length > 0)
                        data.WeightExchange = GetExchangeWeight(pack.MovingMethod, data);

                    //Update tien p
                    var config = db.tbl_Configurations.FirstOrDefault();
                    double minPackage = 0.3;
                    double minOrder = 1;
                    if (config != null)
                    {
                        minPackage = config.MinPackage ?? 0.3;
                        minOrder = config.MinOrder ?? 1;
                    }

                    data.Weight = Converted.ToDouble(data.Weight);

                    var lstFee = db.tbl_FeeWeights.Where(x => x.Type == pack.MovingMethod).ToList();
                    if (lstFee == null || lstFee.Count == 0)
                    {
                        dt.IsError = true;
                        dt.Message = "Chưa thiết lập giá cho loại vận chuyển " + PJUtils.ShippingMethodName(pack.MovingMethod);
                        return dt;
                    }

                    var fee = lstFee.FirstOrDefault(x => x.WeightFrom <= data.Weight && data.Weight <= x.WeightTo);

                    if (lstFee == null || lstFee.Count == 0)
                    {
                        dt.IsError = true;
                        dt.Message = $"Chưa thiết lập giá cho số kilogram {data.Weight}";
                        return dt;
                    }

                    pack.Weight = Converted.ToDouble(data.Weight).ToString();
                    pack.WeightExchange = Converted.ToDouble(data.WeightExchange).ToString();
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

                    if (data.Date != null && data.Date != DateTime.MinValue)
                        pack.ImportedSGWH = data.Date.AddHours(7);

                    if (pack.MovingMethod < 3)
                    {
                        var maxWeight = data.Weight * (25 / (float)100);
                        if (data.WeightExchange > data.Weight || (data.WeightExchange > (data.Weight + maxWeight)) || data.WeightExchange > (data.Weight + 1))
                            pack.WeightReal = data.WeightExchange.ToString();
                        else
                            pack.WeightReal = Converted.ToDouble(data.Weight).ToString();
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

                    db.Update(pack);
                    if (db.SaveChanges() > 0)
                    {
                        if (user != null)
                            AccountBusiness.UpdateSearch(user.ID, user.AvailableSearch + 1);

                        //Ktra xem khach da co don chua.
                        DateTime cnExportDateFrom = DateTime.Now;
                        if (pack.ExportedCNWH != null)
                            cnExportDateFrom = pack.ExportedCNWH.Value.Date;
                        else
                            pack.ExportedCNWH = cnExportDateFrom;

                        DateTime cnExportDateEnd = cnExportDateFrom.AddDays(1).AddTicks(-1);
                        DateTime startDate = pack.ImportedSGWH.Value.Date; //One day 
                        DateTime endDate = startDate.AddDays(1).AddTicks(-1);
                        tbl_ShippingOrder check = null;

                        if (!string.IsNullOrEmpty(pack.TransID))
                            check = ShippingOrder.GetOne(pack.TransID);

                        if (check == null || (check != null && check.Username != pack.Username))
                            check = ShippingOrder.CheckOrderInStock(pack.Username, pack.MovingMethod, startDate, endDate, cnExportDateFrom, cnExportDateEnd);

                        if (check != null)
                        {
                            //Cap nhat lai transid cho p
                            pack.TransID = check.RecID;
                            pack.ModifiedBy = accessor;
                            pack.ModifiedDate = DateTime.Now;
                            UpdateTrans(pack.ID, check.RecID);
                            var lstPack = GetByTransId(check.RecID);

                            double weight = 0;
                            double weightPrice = 0;
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

                            double totalWeight = Converted.ToDouble(weight + weightBrand);

                            var finalfee = lstFee.FirstOrDefault(x => x.WeightFrom <= totalWeight && totalWeight <= x.WeightTo);
                            if (finalfee != null)
                            {
                                if (totalWeight < minOrder)
                                {
                                    if (weight > 0)
                                        weightPrice = Converted.DoubleCeiling((minOrder - weightBrand) * Converted.ToDouble(finalfee.Amount));
                                    else
                                        priceBrand = Converted.DoubleCeiling((Converted.ToDouble(finalfee.PriceBrand) + Converted.ToDouble(finalfee.Amount)) * minOrder);

                                    weight = minOrder;
                                }
                                else
                                {
                                    weightPrice = Converted.DoubleCeiling(weight * Converted.ToDouble(finalfee.Amount));
                                    weight = totalWeight;
                                }
                            }

                            double totalWoodPrice = lstPack.Where(x => x.WoodPackagePrice != null).Sum(x => Converted.ToDouble(x.WoodPackagePrice));
                            double totalAirPrice = lstPack.Where(x => x.AirPackagePrice != null).Sum(x => Converted.ToDouble(x.AirPackagePrice));
                            double totalInsurPrice = lstPack.Where(x => x.IsInsurancePrice != null).Sum(x => Converted.ToDouble(x.IsInsurancePrice));
                            double totalCharge = lstPack.Sum(x => Converted.ToDouble(x.SurCharge));
                            double totalweightPrice = priceBrand + weightPrice;
                            var totalPrice = totalweightPrice + totalWoodPrice + totalAirPrice + totalInsurPrice + totalCharge;
                            var sPackageCode = lstPack.Select(x => x.PackageCode).ToArray();

                            check.Weight = Converted.Double2String(weight);
                            check.WeightPrice = Converted.StringCeiling(totalweightPrice);
                            check.PackageCode = string.Join(", ", sPackageCode);
                            check.WoodPackagePrice = totalWoodPrice.ToString();
                            check.AirPackagePrice = totalAirPrice.ToString();
                            check.InsurancePrice = totalInsurPrice.ToString();
                            check.TotalPrice = totalPrice.ToString();
                            check.SurCharge = totalCharge.ToString();
                            check.ModifiedBy = accessor;
                            check.ModifiedDate = DateTime.Now;
                            db.Update(check);
                            if (db.SaveChanges() > 0)
                            {
                                ShippingOrder.UpdateByPackageChanged(pack, accessor);

                                dt.IsError = false;
                                dt.Message = "Đã nhập kho";
                                return dt;
                            }
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
                            ship.PackageCode = pack.PackageCode;
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
                            db.Add(ship);
                            bool save = db.SaveChanges() > 0;
                            if (save)
                            {
                                var oPack = db.tbl_Packages.FirstOrDefault(x => x.ID == pack.ID);
                                if (oPack != null)
                                {
                                    oPack.TransID = ship.RecID;
                                    oPack.ModifiedBy = accessor;
                                    oPack.ModifiedDate = DateTime.Now;
                                    if (BusinessBase.Update(oPack))
                                        ShippingOrder.UpdateByPackageChanged(oPack, accessor);
                                }


                                NotificationBusiness.Insert(admin.ID, admin.Username, pack.UID, pack.Username, ship.ID, ship.ShippingOrderCode, "Đơn hàng " + ship.ShippingOrderCode + " đã nhập kho HCM", 1, "/ShippingOrder/Details/" + ship.ID, accessor);
                                dt.IsError = false;
                                dt.Message = "Đã nhập kho";
                                return dt;
                            }
                            dt.IsError = true;
                            dt.Message = "Lỗi vui lòng thử lại";
                            return dt;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("InstockSGWare", ex.Message);
                }
                dt.IsError = true;
                dt.Message = "Lỗi vui lòng thử lại";
                return dt;
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
                                    int movingMethod = 1;
                                    int type = Converted.ToInt(dt.Rows[row][2].ToString());
                                    if (type == 3)
                                        movingMethod = 1;
                                    if (type == 5)
                                        movingMethod = 2;
                                    if (type == 8)
                                        movingMethod = 3;
                                    if (type == 15)
                                        movingMethod = 4;
                                    if (type == 20)
                                        movingMethod = 5;
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
                                            oExist.MovingMethod = movingMethod;
                                            oExist.Note = note;
                                            oExist.Status = 3;
                                            oExist.BigPackage = big.ID;
                                            oExist.OrderDate = date;
                                            oExist.ExportedCNWH = date;
                                            oExist.DateExpectation = "Dự kiến " + d.ToString("dd/MM/yyyy");
                                            oExist.ModifiedDate = DateTime.Now;
                                            oExist.ModifiedBy = accesser;
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

                                    p.MovingMethod = movingMethod;
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
                                    _log.Error("Import Excel", ex.Message);
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
                            byte[] fileBytes = File.ReadAllBytes(pathRoot);
                            return pathRoot;
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Export file excel", ex.Message);
                            return "";
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("MarkupValueExcel", ex.Message);
                return "";
            }
        }

        public static DataReturnModel<bool> Cancel(int id, string username)
        {
            using (var db = new nhanshiphangContext())
            {
                var data = new DataReturnModel<bool>();
                var pack = BusinessBase.GetOne<tbl_Package>(x => x.ID == id && x.Status <= 2);
                if (pack != null)
                {
                    pack.Status = 9;
                    pack.ModifiedBy = username;
                    pack.ModifiedDate = DateTime.Now;
                    db.Update(pack);
                    if (db.SaveChanges() > 0)
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
        }
        public static DataReturnModel<bool> Delete(int id)
        {
            using (var db = new nhanshiphangContext())
            {
                var data = new DataReturnModel<bool>();
                var pack = BusinessBase.GetOne<tbl_Package>(x => x.ID == id && x.Status <= 2);
                if (pack != null)
                {
                    db.Remove(pack);
                    if (db.SaveChanges() > 0)
                    {
                        data.IsError = false;
                        data.Message = "Xóa đơn thành công!";
                        return data;
                    }
                }
                data.IsError = true;
                data.Message = "Xóa không thành công, vui lòng liên hệ với nhân viên để được giải quyết !";
                return data;
            }
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
