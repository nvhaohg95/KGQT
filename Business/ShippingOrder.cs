using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace KGQT.Business
{
    public static class ShippingOrder
    {
        private static readonly ILogger _log = Log.ForContext(typeof(ShippingOrder));
        #region Get count
        public static tbl_ShippingOrder GetOne(int id)
        {
            using (var db = new nhanshiphangContext())
            {
                return db.tbl_ShippingOrders.FirstOrDefault(x => x.ID == id);
            }
        }
        public static tbl_ShippingOrder GetOne(string recID)
        {
            using (var db = new nhanshiphangContext())
            {
                return db.tbl_ShippingOrders.FirstOrDefault(x => x.RecID == recID);
            }
        }

        public static tbl_ShippingOrder CheckOrderInStock(string username, int method, DateTime fromDate, DateTime toDate, DateTime exportStart, DateTime exportEnd, string recID = "")
        {
            using (var db = new nhanshiphangContext())
            {
                try
                {
                    var query = db.tbl_ShippingOrders.Where(x => x.Status == 1 && x.ShippingMethod == method && x.Username.ToLower() == username.ToLower()
                    && (x.CreatedDate >= fromDate && x.CreatedDate < toDate) && (x.ChinaExportDate >= exportStart && x.ChinaExportDate < exportEnd));

                    if (!string.IsNullOrEmpty(recID))
                        query = query.Where(x => x.RecID != recID);

                    return query.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        public static object[] GetAllStatus(string username)
        {
            using (var db = new nhanshiphangContext())
            {
                IQueryable<tbl_ShippingOrder> qry = db.tbl_ShippingOrders;
                if (!string.IsNullOrEmpty(username))
                    qry = qry.Where(x => x.Username == username);

                int st1 = qry.Where(x => x.Status == 1).Count();
                int st2 = qry.Where(x => x.Status == 2).Count();
                int total = qry.Count();
                return new object[] { st1, st2, total };
            }
        }
        #endregion

        #region Get List
        public static (int, double) GetTotalAndLeft(string username)
        {
            using (var db = new nhanshiphangContext())
            {
                IQueryable<tbl_ShippingOrder> queryable = db.tbl_ShippingOrders.Where(x => x.Username == username);
                int total = queryable.Count();
                double totalLeft = queryable.Where(x => x.Status == 1 || x.Status == 0).AsEnumerable().Sum(x => x.TotalPrice.Double());
                return (total, totalLeft);
            }
        }
        public static object[] GetPage(int status, string ID, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20, string? userName = "")
        {

            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tbl_ShippingOrder>();
                int totalPage = 1;
                int total = 0;
                IQueryable<tbl_ShippingOrder> query = db.tbl_ShippingOrders;
                if (!string.IsNullOrEmpty(userName))
                    query = query.Where(x => x.Username == userName);

                if (status > 0)
                    query = query.Where(x => x.Status == status);

                if (!string.IsNullOrEmpty(ID))
                    query = query.Where(x => x.ShippingOrderCode.Contains(ID) || x.Username.Contains(ID) || x.PackageCode.Contains(ID));

                if (fromDate != null)
                    query = query.Where(x => x.CreatedDate >= fromDate);

                if (toDate != null)
                    query = query.Where(x => x.CreatedDate <= toDate);

                total = query.Count();
                if (total > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));

                    lstData = query.OrderByDescending(x => x.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize).ToList();

                    var lstTrans = lstData.Select(x => x.RecID).ToArray();
                    var packs = db.tbl_Packages.Where(x => lstTrans.Contains(x.TransID)).ToList();
                    foreach (var item in lstData)
                    {
                        List<string> pack = packs.Where(x => x.TransID == item.RecID).Select(x => x.PackageCode + " - " + x.WeightReal + "kg").ToList();
                        item.Packages = pack;
                    }
                }

                return new object[] { lstData, total, totalPage };
            }
        }

        #endregion

        #region CRUD
        public static bool Add(tbl_ShippingOrder model)
        {
            return BusinessBase.Add(model);
        }

        public static bool Add(tbl_Package pack, string accessor)
        {
            using (var db = new nhanshiphangContext())
            {
                var config = db.tbl_Configurations.FirstOrDefault();
                double minPackage = 0.3;
                double minOrder = 1;
                if (config != null)
                {
                    minPackage = config.MinPackage ?? 0.3;
                    minOrder = config.MinOrder ?? 1;
                }

                var weightRound = Converted.ToDouble(pack.WeightReal);

                if (weightRound < minOrder)
                    weightRound = minOrder;

                var lstFee = db.tbl_FeeWeights.Where(x => x.Type == pack.MovingMethod).ToList();
                var fee = lstFee.FirstOrDefault(x => x.WeightFrom <= weightRound && weightRound <= x.WeightTo);
                double feeWeight = Converted.DoubleCeiling(weightRound * Converted.ToDouble(fee.Amount));

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
                    + Converted.ToDouble(pack.MoreCharge)
                    + Converted.ToDouble(pack.SurCharge));
                ship.Status = 1;
                ship.ChinaExportDate = pack.ExportedCNWH;
                ship.CreatedDate = DateTime.Now;
                ship.CreatedBy = accessor;
                db.Add(ship);
                var save = db.SaveChanges() > 0;
                if (save)
                {
                    var oPack = db.tbl_Packages.FirstOrDefault(x => x.ID == pack.ID);
                    if (oPack != null)
                    {
                        oPack.TransID = ship.RecID;
                        oPack.ModifiedBy = accessor;
                        oPack.ModifiedDate = DateTime.Now;
                        BusinessBase.Update(oPack);
                    }
                    var admin = db.tbl_Accounts.FirstOrDefault(x => x.Username == accessor);

                    string message = "Đơn hàng " + ship.ShippingOrderCode + " đã nhập kho HCM";
                    NotificationBusiness.Insert(admin.ID, admin.Username, pack.UID, pack.Username, ship.ID, ship.ShippingOrderCode, message, message, 1, "/ShippingOrder/Details/" + ship.ID, accessor);
                }
                return save;
            }
        }
        #endregion

        #region 
        public static void CalculatorAllPrice(string transID, string accessor)
        {
            if (string.IsNullOrEmpty(transID)) return;
            using (var db = new nhanshiphangContext())
            {
                var ship = db.tbl_ShippingOrders.FirstOrDefault(x => x.RecID == transID);

                if (ship == null) return;

                var packages = db.tbl_Packages.Where(x => x.TransID == ship.RecID).ToList();
                if (packages.Count == 0)
                {
                    db.Remove(ship);
                    db.SaveChanges();
                    return;
                }

                var config = db.tbl_Configurations.FirstOrDefault();
                var lstFee = db.tbl_FeeWeights.Where(x => x.Type == ship.ShippingMethod).ToList();
                if (lstFee == null || lstFee.Count == 0) return;
                double minPackage = 0.3;
                double minOrder = 1;
                double weight = 0;
                double weightPrice = 0;
                double priceOver = 0;
                double weightOver = 0;
                double priceBrand = 0;
                double weightBrand = 0;
                foreach (var item in packages)
                {
                    if (item.IsBrand == true && item.WeightReal.Double() < 500)
                        weightBrand += Converted.ToDouble(item.WeightReal);
                    else if (weight < 500)
                        weight += Converted.ToDouble(item.WeightReal);
                    else
                    {
                        weightOver += Converted.ToDouble(item.WeightReal);
                        priceOver += item.TotalPrice.Double();
                    }
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

                weight += weightOver;


                double totalWoodPrice = packages.Where(x => x.WoodPackagePrice != null).Sum(x => Converted.ToDouble(x.WoodPackagePrice));
                double totalAirPrice = packages.Where(x => x.AirPackagePrice != null).Sum(x => Converted.ToDouble(x.AirPackagePrice));
                double totalInsurPrice = packages.Where(x => x.IsInsurancePrice != null).Sum(x => Converted.ToDouble(x.IsInsurancePrice));
                double totalCharge = packages.Sum(x => Converted.ToDouble(x.SurCharge));
                double totalMoreCharge = packages.Sum(x => Converted.ToDouble(x.MoreCharge));
                double totalweightPrice = priceBrand + weightPrice + priceOver;
                var totalPrice = totalweightPrice + totalWoodPrice + totalAirPrice + totalInsurPrice + totalCharge + totalMoreCharge;
                var sPack = packages.Select(x => x.PackageCode).ToArray();

                ship.PackageCode = string.Join(", ", sPack);
                ship.Weight = Converted.Double2String(weight);
                ship.WeightPrice = Converted.StringCeiling(totalweightPrice);
                ship.WoodPackagePrice = totalWoodPrice.ToString();
                ship.AirPackagePrice = totalAirPrice.ToString();
                ship.InsurancePrice = totalInsurPrice.ToString();
                ship.TotalPrice = totalPrice.ToString();
                ship.SurCharge = totalCharge.ToString();
                ship.MoreCharge = totalMoreCharge.ToString();
                ship.ModifiedBy = accessor;
                ship.ModifiedDate = DateTime.Now;
                db.tbl_ShippingOrders.Update(ship);
                db.SaveChanges();
                return;
            }
        }

        public static void UpdateByPackageChanged(tbl_Package p, string accessor)
        {
            using (var db = new nhanshiphangContext())
            {
                var lstOrder = db.tbl_ShippingOrders.Where(x => x.PackageCode.Contains(p.PackageCode)).ToList();
                foreach (var item in lstOrder)
                {
                    if (item.RecID != p.TransID)
                        CalculatorAllPrice(item.RecID, accessor);
                }
                return;
            }
        }

        public static DataReturnModel<bool> Payment(int id, string accessor)
        {
            using (var db = new nhanshiphangContext())
            {
                var dt = new DataReturnModel<bool>();
                var oOrder = db.tbl_ShippingOrders.FirstOrDefault(x => x.ID == id);
                var oldOrder = db.tbl_ShippingOrders.FirstOrDefault(x => x.ID == id);
                if (oOrder == null)
                {
                    dt.IsError = true;
                    dt.Message = "Không tìm thấy thông tin đơn";
                    return dt;
                }

                if (oOrder.Status == 2)
                {

                    dt.IsError = true;
                    dt.Message = "Đơn này đã thanh toán rồi";
                    return dt;
                }

                var oUser = db.tbl_Accounts.FirstOrDefault(x => x.Username == oOrder.Username);
                var oldUser = db.tbl_Accounts.FirstOrDefault(x => x.Username == oOrder.Username);

                if (oUser == null)
                {
                    dt.IsError = true;
                    dt.Message = "Không tìm thấy thông tin khách hàng";
                    return dt;
                }

                try
                {
                    double totalPrice = Convert.ToDouble(oOrder.TotalPrice);
                    double wallet = Convert.ToDouble(oUser.Wallet);
                    if (wallet >= totalPrice)
                    {
                        oOrder.Status = 2;
                        oOrder.ModifiedBy = accessor;
                        oOrder.ModifiedDate = DateTime.Now;
                        db.tbl_ShippingOrders.Update(oOrder);
                        if (db.SaveChanges() > 0)
                        {

                            var u = db.tbl_Accounts.FirstOrDefault(x => x.ID == oUser.ID);
                            var moneyPrevious = Converted.Double2Money(wallet);
                            var pay = Converted.StringCeiling(wallet - totalPrice);
                            u.Wallet = pay;
                            db.Update(u);
                            var save = db.SaveChanges() > 0;
                            if (!save)
                            {
                                dt.IsError = true;
                                dt.Message = "Thanh toán thất bại. Vui lòng kiểm tra lại!";
                                return dt;
                            }
                            HistoryPayWallet.Insert(oUser.ID, oUser.Username, oOrder.ID, $"Thanh toán cho đơn {oOrder.ShippingOrderCode}", oOrder.TotalPrice, 1, 1, moneyPrevious, pay, accessor);
                            var packs = db.tbl_Packages.Where(x => x.TransID == oOrder.ShippingOrderCode).ToList();
                            foreach (var pack in packs)
                            {
                                pack.Status = 5;
                                pack.ModifiedBy = accessor;
                                pack.ModifiedDate = DateTime.Now;
                                db.Update(pack);
                                BusinessBase.TrackLog(oUser.ID, pack.ID, "{0} đã thanh toán cho kiện {1}", 1, oOrder.Username);
                            }

                            db.SaveChanges();
                            dt.IsError = false;
                            dt.Message = "Thanh toán thành công.";
                            return dt;
                        }
                    }
                    else
                    {
                        var pay = Converted.StringCeiling(totalPrice - wallet);
                        dt.IsError = true;
                        dt.Message = $"Tài khoản {oOrder.Username} không đủ tiền, cần phải nạp thêm {Converted.String2Money(pay)}đ";
                        return dt;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Payment", ex.Message);
                    db.tbl_ShippingOrders.Update(oldOrder);
                    db.tbl_Accounts.Update(oldUser);
                    db.SaveChanges();
                }
                dt.IsError = true;
                dt.Message = "Lỗi, vui lòng tải lại trang!";
                return dt;
            }
        }

        public static DataReturnModel<bool> PaymentDirect(int id, string accessor)
        {
            using (var db = new nhanshiphangContext())
            {
                var dt = new DataReturnModel<bool>();
                var oOrder = db.tbl_ShippingOrders.FirstOrDefault(x => x.ID == id);
                var oldOrder = db.tbl_ShippingOrders.FirstOrDefault(x => x.ID == id);
                if (oOrder == null)
                {
                    dt.IsError = true;
                    dt.Message = "Không tìm thấy thông tin đơn";
                    return dt;
                }

                if (oOrder.Status == 2)
                {

                    dt.IsError = true;
                    dt.Message = "Đơn này đã thanh toán rồi";
                    return dt;
                }

                var oUser = db.tbl_Accounts.FirstOrDefault(x => x.Username == oOrder.Username);

                if (oUser == null)
                {
                    dt.IsError = true;
                    dt.Message = "Không tìm thấy thông tin khách hàng";
                    return dt;
                }

                try
                {
                    double totalPrice = Convert.ToDouble(oOrder.TotalPrice);

                    oOrder.Status = 2;
                    oOrder.ModifiedBy = accessor;
                    oOrder.ModifiedDate = DateTime.Now;
                    db.tbl_ShippingOrders.Update(oOrder);
                    if (db.SaveChanges() > 0)
                    {
                        HistoryPayWallet.Insert(oUser.ID, oUser.Username, oOrder.ID, $"Thanh toán cho đơn {oOrder.ShippingOrderCode}", oOrder.TotalPrice, 1, 1, oUser.Wallet, oUser.Wallet, accessor);
                        var packs = db.tbl_Packages.Where(x => x.TransID == oOrder.ShippingOrderCode).ToList();
                        foreach (var pack in packs)
                        {
                            pack.Status = 5;
                            pack.ModifiedBy = accessor;
                            pack.ModifiedDate = DateTime.Now;
                            db.Update(pack);
                            BusinessBase.TrackLog(oUser.ID, pack.ID, "{0} đã thanh toán cho kiện {1}", 1, oOrder.Username);
                        }

                        db.SaveChanges();
                        dt.IsError = false;
                        dt.Message = "Thanh toán thành công.";
                        return dt;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Payment", ex.Message);
                    db.tbl_ShippingOrders.Update(oldOrder);
                    db.SaveChanges();
                }
                dt.IsError = true;
                dt.Message = "Lỗi, vui lòng tải lại trang!";
                return dt;
            }
        }
        public static DataReturnModel<bool> PaymentSelected(string data, string accessor)
        {
            using (var db = new nhanshiphangContext())
            {
                var dt = new DataReturnModel<bool>();
                var lst = data.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var ships = db.tbl_ShippingOrders.Where(x => lst.Contains(x.RecID) && x.Username == accessor).ToList();

                var totalMoney = ships.Sum(x => x.TotalPrice.Double());
                var userWallet = db.tbl_Accounts.FirstOrDefault(x => x.Username == accessor);
                if (userWallet == null)
                {
                    dt.IsError = true;
                    dt.Message = "Bạn chưa đăng nhập hoặc phiên đăng nhập đã hết hạn";
                    return dt;
                }

                if (userWallet.Wallet.Double() < totalMoney)
                {
                    dt.IsError = true;
                    dt.Message = $"Bạn cần phải nạp thêm {Converted.Double2Money(totalMoney - userWallet.Wallet.Double())} để thanh toán đơn hàng";
                    return dt;
                }
                List<string> msg = new List<string>();
                foreach (var item in ships)
                {
                    var oldOrder = item;
                    if (item.Status == 2)
                        continue;

                    var oUser = db.tbl_Accounts.FirstOrDefault(x => x.Username == item.Username);
                    var oldUser = db.tbl_Accounts.FirstOrDefault(x => x.Username == item.Username);

                    try
                    {
                        double totalPrice = Convert.ToDouble(item.TotalPrice);
                        double wallet = Convert.ToDouble(oUser.Wallet);
                        if (wallet >= totalPrice)
                        {
                            item.Status = 2;
                            item.ModifiedBy = accessor;
                            item.ModifiedDate = DateTime.Now;
                            db.tbl_ShippingOrders.Update(item);
                            if (db.SaveChanges() > 0)
                            {
                                var u = db.tbl_Accounts.FirstOrDefault(x => x.ID == oUser.ID);
                                var moneyPrevious = Converted.Double2Money(wallet);
                                var pay = Converted.StringCeiling(wallet - totalPrice);
                                u.Wallet = pay;
                                db.Update(u);
                                var save = db.SaveChanges() > 0;
                                if (!save)
                                {
                                    dt.IsError = true;
                                    msg.Add($"Thanh toán đơn {item.ShippingOrderCode} thất bại!");
                                    continue;
                                }
                                HistoryPayWallet.Insert(oUser.ID, oUser.Username, item.ID, $"Thanh toán cho đơn {item.ShippingOrderCode}", item.TotalPrice, 1, 1, moneyPrevious, pay, accessor);
                                var packs = db.tbl_Packages.Where(x => x.TransID == item.RecID).ToList();
                                foreach (var pack in packs)
                                {
                                    pack.Status = 5;
                                    pack.ModifiedBy = accessor;
                                    pack.ModifiedDate = DateTime.Now;
                                    db.Update(pack);
                                    BusinessBase.TrackLog(oUser.ID, pack.ID, "{0} đã thanh toán cho kiện {1}", 1, item.Username);
                                }
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var pay = Converted.StringCeiling(totalPrice - wallet);
                            dt.IsError = true;
                            dt.Message = $"Tài khoản {item.Username} không đủ tiền, cần phải nạp thêm {Converted.String2Money(pay)}đ";
                            return dt;
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Payment", ex.Message);
                        db.tbl_ShippingOrders.Update(oldOrder);
                        db.tbl_Accounts.Update(oldUser);
                        db.SaveChanges();
                        dt.IsError = true;
                        msg.Add($"Thanh toán đơn {item.ShippingOrderCode} thất bại!");
                    }
                }
                dt.IsError = msg.Count > 0 ? true : false;
                dt.Message = msg.Count > 0 ? string.Join("</br>", msg) : "Thanh toán đơn hàng thành công";
                return dt;
            }
        }

        public static DataReturnModel<bool> PaymentAll(string accessor)
        {
            using (var db = new nhanshiphangContext())
            {
                var dt = new DataReturnModel<bool>();
                var ships = db.tbl_ShippingOrders.Where(x => x.Username == accessor && x.Status < 2).ToList();

                var totalMoney = ships.Sum(x => x.TotalPrice.Double());
                var userWallet = db.tbl_Accounts.FirstOrDefault(x => x.Username == accessor);
                if (userWallet == null)
                {
                    dt.IsError = true;
                    dt.Message = "Bạn chưa đăng nhập hoặc phiên đăng nhập đã hết hạn";
                    return dt;
                }

                if (userWallet.Wallet.Double() < totalMoney)
                {
                    dt.IsError = true;
                    dt.Message = $"Bạn cần phải nạp thêm {Converted.Double2Money(totalMoney - userWallet.Wallet.Double())} để thanh toán đơn hàng";
                    return dt;
                }
                List<string> msg = new List<string>();
                foreach (var item in ships)
                {
                    var oldOrder = item;
                    if (item.Status == 2)
                        continue;

                    var oUser = db.tbl_Accounts.FirstOrDefault(x => x.Username == item.Username);
                    var oldUser = db.tbl_Accounts.FirstOrDefault(x => x.Username == item.Username);

                    try
                    {
                        double totalPrice = Convert.ToDouble(item.TotalPrice);
                        double wallet = Convert.ToDouble(oUser.Wallet);
                        if (wallet >= totalPrice)
                        {
                            item.Status = 2;
                            item.ModifiedBy = accessor;
                            item.ModifiedDate = DateTime.Now;
                            db.tbl_ShippingOrders.Update(item);
                            if (db.SaveChanges() > 0)
                            {
                                var u = db.tbl_Accounts.FirstOrDefault(x => x.ID == oUser.ID);
                                var moneyPrevious = Converted.Double2Money(wallet);
                                var pay = Converted.StringCeiling(wallet - totalPrice);
                                u.Wallet = pay;
                                db.Update(u);
                                var save = db.SaveChanges() > 0;
                                if (!save)
                                {
                                    dt.IsError = true;
                                    msg.Add($"Thanh toán đơn {item.ShippingOrderCode} thất bại!");
                                    continue;
                                }
                                HistoryPayWallet.Insert(oUser.ID, oUser.Username, item.ID, $"Thanh toán cho đơn {item.ShippingOrderCode}", item.TotalPrice, 1, 1, moneyPrevious, pay, accessor);
                                var packs = db.tbl_Packages.Where(x => x.TransID == item.RecID).ToList();
                                foreach (var pack in packs)
                                {
                                    pack.Status = 5;
                                    pack.ModifiedBy = accessor;
                                    pack.ModifiedDate = DateTime.Now;
                                    db.Update(pack);
                                    BusinessBase.TrackLog(oUser.ID, pack.ID, "{0} đã thanh toán cho kiện {1}", 1, item.Username);
                                }
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var pay = Converted.StringCeiling(totalPrice - wallet);
                            dt.IsError = true;
                            dt.Message = $"Tài khoản {item.Username} không đủ tiền, cần phải nạp thêm {Converted.String2Money(pay)}đ";
                            return dt;
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Payment", ex.Message);
                        db.tbl_ShippingOrders.Update(oldOrder);
                        db.tbl_Accounts.Update(oldUser);
                        db.SaveChanges();
                        dt.IsError = true;
                        msg.Add($"Thanh toán đơn {item.ShippingOrderCode} thất bại!");
                    }
                }
                dt.IsError = msg.Count > 0 ? true : false;
                dt.Message = msg.Count > 0 ? string.Join("</br>", msg) : "Thanh toán đơn hàng thành công";
                return dt;
            }
        }

        public static DataReturnModel<bool> Cancel(int id, string accsseor)
        {
            using (var db = new nhanshiphangContext())
            {
                var dt = new DataReturnModel<bool>();
                var order = db.tbl_ShippingOrders.FirstOrDefault(x => x.ID == id);
                if (order == null)
                {
                    dt.IsError = true;
                    dt.Message = "Không tìm thấy thông tin đơn";
                    return dt;
                }
                order.Status = 9;
                order.ModifiedDate = DateTime.Now;
                order.ModifiedBy = accsseor;
                db.Update(order);
                try
                {
                    if (db.SaveChanges() > 0)
                    {
                        dt.IsError = false;
                        dt.Message = "Đã hủy đơn";
                        return dt;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Cancel", ex.Message);
                }
                dt.IsError = true;
                dt.Message = "Lỗi, vui lòng tải lại trang!";
                return dt;
            }
        }


        public static DataReturnModel<bool> Receiver(int id, string accsseor)
        {
            using (var db = new nhanshiphangContext())
            {
                var dt = new DataReturnModel<bool>();
                var oOrder = db.tbl_ShippingOrders.FirstOrDefault(x => x.ID == id);
                if (oOrder == null)
                {
                    dt.IsError = true;
                    dt.Message = "Không tìm thấy thông tin đơn";
                    return dt;
                }

                if (oOrder.Status == 2)
                {
                    dt.IsError = true;
                    dt.Message = "Đơn này đã nhận rồi";
                    return dt;
                }

                oOrder.Status = 2;
                oOrder.ModifiedBy = accsseor;
                oOrder.ModifiedDate = DateTime.Now;
                try
                {
                    db.Update(oOrder);
                    if (db.SaveChanges() > 0)
                    {
                        var packs = db.tbl_Packages.Where(x => x.TransID == oOrder.ShippingOrderCode).ToList();
                        foreach (var pack in packs)
                        {
                            pack.Status = 5;
                            pack.ModifiedBy = accsseor;
                            pack.ModifiedDate = DateTime.Now;
                            db.Update(pack);
                        }
                        db.SaveChanges();
                        dt.IsError = false;
                        dt.Message = "Nhận hàng thành công!";
                        return dt;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Receiver order", ex.Message);
                }

                dt.IsError = true;
                dt.Message = "Lỗi, vui lòng tải lại trang!";
                return dt;
            }
        }
        #endregion
    }
}
