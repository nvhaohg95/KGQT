﻿using KGQT.Business.Base;
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

        public static tbl_ShippingOrder CheckOrderInStock(string username, int method, DateTime fromDate, DateTime toDate, DateTime exportStart, DateTime exportEnd)
        {
            using (var db = new nhanshiphangContext())
            {
                try
                {
                    var query = db.tbl_ShippingOrders.Where(x => x.Status == 1 && x.ShippingMethod == method && x.Username.ToLower() == username.ToLower()
                    && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.ChinaExportDate >= exportStart && x.ChinaExportDate <= exportEnd);

                    var a = query.FirstOrDefault();
                    return a;
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
                    query = query.Where(x => x.ShippingOrderCode.Contains(ID) || x.Username.Contains(ID));

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
                var config = db.tbl_Configurations.FirstOrDefault();
                var lstFee = db.tbl_FeeWeights.Where(x => x.Type == ship.ShippingMethod).ToList();
                if (lstFee == null || lstFee.Count == 0) return;
                double minPackage = 0.3;
                double minOrder = 1;
                double weight = 0;
                double weightPrice = 0;
                double priceBrand = 0;
                double weightBrand = 0;
                foreach (var item in packages)
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

                double totalWoodPrice = packages.Where(x => x.WoodPackagePrice != null).Sum(x => Converted.ToDouble(x.WoodPackagePrice));
                double totalAirPrice = packages.Where(x => x.AirPackagePrice != null).Sum(x => Converted.ToDouble(x.AirPackagePrice));
                double totalInsurPrice = packages.Where(x => x.IsInsurancePrice != null).Sum(x => Converted.ToDouble(x.IsInsurancePrice));
                double totalCharge = packages.Sum(x => Converted.ToDouble(x.SurCharge));
                double totalweightPrice = priceBrand + weightPrice;
                var totalPrice = totalweightPrice + totalWoodPrice + totalAirPrice + totalInsurPrice + totalCharge;
                ship.Weight = Converted.Double2String(weight);
                ship.WeightPrice = Converted.StringCeiling(totalweightPrice);
                ship.WoodPackagePrice = totalWoodPrice.ToString();
                ship.AirPackagePrice = totalAirPrice.ToString();
                ship.InsurancePrice = totalInsurPrice.ToString();
                ship.TotalPrice = totalPrice.ToString();
                ship.SurCharge = totalCharge.ToString();
                ship.ModifiedBy = accessor;
                ship.ModifiedDate = DateTime.Now;
                db.tbl_ShippingOrders.Update(ship);
                db.SaveChanges();
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

                            HistoryPayWallet.Insert(oUser.ID, oUser.Username, oOrder.ID, $"Thanh toán cho đơn {oOrder.ShippingOrderCode}", oOrder.TotalPrice, 1, 1, pay, accessor);
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
