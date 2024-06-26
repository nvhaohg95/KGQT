﻿using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Spreadsheet;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using System;
using Serilog;
using ILogger = Serilog.ILogger;
using Newtonsoft.Json;

namespace KGQT.Business
{
    public class WithDrawBusiness
    {
        private static readonly ILogger _log = Log.ForContext(typeof(NotificationBusiness));

        public static object[] GetList(int ID,int type, string userName,int status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            var lstData = new List<tbl_Withdraw>();
            int total = 0;
            int totalPage = 0;
            using (var db = new nhanshiphangContext())
            {
                var query = db.tbl_Withdraws.AsQueryable();
                if(ID > 0)
                    query = query.Where(x => x.ID == ID);
                if (type != 0)
                    query = query.Where(x => x.Type == type);
                if (!string.IsNullOrEmpty(userName))
                    query = query.Where(x => x.Username != null && x.Username.Contains(userName));
                if (status != 0)
                    query = query.Where(x => x.Status == status);
                if (fromDate != null)
                    query = query.Where(x => x.CreatedDate >= fromDate);
                if (toDate != null)
                {
                    toDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(x => x.CreatedDate < toDate);
                }
                total = query.Count();
                if (total > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                    lstData = query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }
            }
            return new object[] { lstData, total, totalPage };
        }

        public static DataReturnModel<bool> Insert(tbl_Withdraw data,string createdBy)
        {
            DataReturnModel<bool> result = new DataReturnModel<bool>();
            try
            {
                if(data.PaymentMethod == 2)
                {
                    if(string.IsNullOrEmpty(data.BankName))
                    {
                        result.IsError = true;
                        result.Data = false;
                        result.Message = "Thông tin ngân hàng không được bỏ trống!";
                        return result;
                    }
                    else if (string.IsNullOrEmpty(data.AccountNumber))
                    {
                        result.IsError = true;
                        result.Data = false;
                        result.Message = "Số tài khoản không được bỏ trống!";
                        return result;
                    }
                    else if (string.IsNullOrEmpty(data.AccountName))
                    {
                        result.IsError = true;
                        result.Data = false;
                        result.Message = "Chủ tài khoản không được bỏ trống!";
                        return result;
                    }
                }    
                if (Converted.ToDouble(data.Amount) <= 0)
                {
                    result.IsError = true;
                    result.Data = false;
                    result.Message = "Số tiền không được bỏ trống!";
                    return result;
                }
                if (string.IsNullOrEmpty((data.Note)))
                {
                    result.IsError = true;
                    result.Data = false;
                    result.Message = "Vui lòng nhập nội dung!";
                    return result;
                }
                using (var db = new nhanshiphangContext())
                {
                    var user = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.Username.ToLower());
                    var admin = db.tbl_Accounts.FirstOrDefault(x => x.Username == createdBy.ToLower());
                    if (user != null && admin != null)
                    {
                        if (data.Type == 2 && Converted.ToDouble(user.Wallet) < Converted.ToDouble(data.Amount))
                        {
                            result.IsError = true;
                            result.Data = false;
                            result.Message = "Số dư trong ví không đủ!";
                            return result;
                        }
                        db.Add(data);
                        int kq = db.SaveChanges();
                        if (kq > 0)
                        {
                            string notiMessage = "";
                            string notiMessage2 = "";
                            if (data.Status == 2)
                            {
                                string strHTML = Converted.Double2Money(Converted.ToDouble(data.Amount));
                                if (data.Type == 1)
                                {
                                    string moneyPrevious = user.Wallet;
                                    string moneyLef = Converted.Double2Money(Converted.ToDouble(user.Wallet) + Converted.ToDouble(data.Amount));
                                    user.Wallet = moneyLef;
                                    user.ModifiedBy = admin.Username;
                                    user.ModifiedDate = DateTime.Now;
                                    db.Update(user);
                                    db.SaveChanges();
                                    notiMessage = string.Format("Tài khoản của Quý khách đã được <span class=\"text-nsh2\">+{0}</span>", strHTML);
                                    notiMessage2 = string.Format("Tài khoản của Quý khách đã được +{0}", strHTML);
                                    HistoryPayWallet.Insert(user.ID, user.Username, data.ID, data.Note, data.Amount, 2, 3,moneyPrevious, moneyLef, createdBy);
                                    NotificationBusiness.Insert(admin.ID, admin.FullName, user.ID, user.FullName, data.ID, "", notiMessage, notiMessage2, 2, "", admin.Username);
                                }
                                else if (data.Type == 2)
                                {
                                    string moneyPrevious = user.Wallet;
                                    string moneyLef = Converted.Double2Money(Converted.ToDouble(user.Wallet) - Converted.ToDouble(data.Amount));
                                    user.Wallet = moneyLef;
                                    user.ModifiedBy = createdBy;
                                    user.ModifiedDate = DateTime.Now;
                                    db.Update(user);
                                    db.SaveChanges();
                                    notiMessage = string.Format("Yêu cầu rút <span class=\"text-nsh1\">{0}</span> của Quý khách đã được duyệt.", strHTML);
                                    notiMessage2 = string.Format("Yêu cầu rút {0} của Quý khách đã được duyệt.", strHTML);
                                    HistoryPayWallet.Insert(user.ID, user.Username, data.ID, data.Note, data.Amount, 1, 4, moneyPrevious, moneyLef, createdBy);
                                    NotificationBusiness.Insert(admin.ID, admin.FullName, user.ID, user.FullName, data.ID, "", notiMessage, notiMessage2, 3, "", admin.Username);
                                }
                                result.IsError = false;
                                result.Message = "Tạo thành công!";
                                result.Data = true;
                            }
                            else
                            {
                                string url = "/Admin/Withdraw/Index?ID=" + data.ID;
                                string money = Converted.Double2Money(Converted.ToDouble(data.Amount));
                                notiMessage = string.Format("Khách hàng <span class=\"fw-bold\">{0}</span> yêu cầu nạp <span class=\"text-nsh2\">{1}</span> vào tài khoản.", string.IsNullOrEmpty(user.FullName) ? user.Username : user.FullName, money);
                                notiMessage2 = string.Format("Khách hàng {0} yêu cầu nạp {1} vào tài khoản.", string.IsNullOrEmpty(user.FullName) ? user.Username : user.FullName, money);
                                NotificationBusiness.Insert(user.ID, user.FullName, 0, "Admin", data.ID, "", notiMessage, notiMessage2, 2, url, user.Username, true);
                                result.IsError = false;
                                result.Data = true;
                                result.Message = "Yêu cầu của Quý khách đã được gửi!";
                            }
                            return result;
                        }
                        else
                        {
                            result.IsError = true;
                            result.Data = false;
                            result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                            return result;
                        }    
                    }
                    else
                    {
                        result.IsError = true;
                        result.Data = false;
                        result.Message = "Không tìm thấy thông tin khách hàng. Vui lòng thử lại sau!";
                        return result;
                    }    
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi tạo lệnh nạp tiền: ", JsonConvert.SerializeObject(ex));
                result.IsError = true;
                result.Data = false;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                return result;
            }
        }
        
        public static DataReturnModel<bool> Insert2(tbl_Withdraw data, string createdBy)
        {
            DataReturnModel<bool> result = new DataReturnModel<bool>();
            try
            {
                if (data.PaymentMethod == 2)
                {
                    if (string.IsNullOrEmpty(data.BankName))
                    {
                        result.IsError = true;
                        result.Data = false;
                        result.Message = "Thông tin ngân hàng không được bỏ trống!";
                        return result;
                    }
                    else if (string.IsNullOrEmpty(data.AccountNumber))
                    {
                        result.IsError = true;
                        result.Data = false;
                        result.Message = "Số tài khoản không được bỏ trống!";
                        return result;
                    }
                    else if (string.IsNullOrEmpty(data.AccountName))
                    {
                        result.IsError = true;
                        result.Data = false;
                        result.Message = "Chủ tài khoản không được bỏ trống!";
                        return result;
                    }
                }
                if (Converted.ToDouble(data.Amount) <= 0)
                {
                    result.IsError = true;
                    result.Data = false;
                    result.Message = "Số tiền không được bỏ trống!";
                    return result;
                }
                if (string.IsNullOrEmpty(data.Note))
                {
                    result.IsError = true;
                    result.Data = false;
                    result.Message = "Vui lòng nhập nội dung!";
                    return result;
                }
                using (var db = new nhanshiphangContext())
                {
                    var user = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.Username.ToLower());
                    if (user != null)
                    {
                        if (Converted.ToDouble(data.Amount) <= 0)
                        {
                            result.IsError = true;
                            result.Data = false;
                            result.Message = "Vui lòng nhập số tiền!";
                            return result;
                        }
                        if (Converted.ToDouble(user.Wallet) < Converted.ToDouble(data.Amount))
                        {
                            result.IsError = true;
                            result.Data = false;
                            result.Message = "Số dư trong ví không đủ!";
                            return result;
                        }
                        string moneyPrevious = user.Wallet;
                        string moneyLef = Converted.Double2Money(Converted.ToDouble(user.Wallet) - Converted.ToDouble(data.Amount));
                        user.Wallet = moneyLef;
                        user.ModifiedBy = createdBy;
                        user.ModifiedDate = DateTime.Now;
                        db.Update(user);
                        db.Add(data);
                        int kq = db.SaveChanges();
                        if (kq > 0)
                        {
                            string url = "/Admin/WithDraw/Refuse?ID=" + data.ID;
                            string money = Converted.Double2Money(Converted.ToDouble(data.Amount));
                            string notiMessage = string.Format("Khách hàng <span class=\"fw-bold\">{0}</span> yêu cầu rút <span class=\"text-nsh1\">{1}</span>.", string.IsNullOrEmpty(user.FullName) ? user.Username : user.FullName, money);
                            string notiMessage2 = string.Format("Khách hàng {0} yêu cầu rút {1}.", string.IsNullOrEmpty(user.FullName) ? user.Username : user.FullName, money);
                            HistoryPayWallet.Insert(user.ID, user.Username, data.ID, data.Note, data.Amount, 1, 4, moneyPrevious, moneyLef, createdBy, 0,0);
                            NotificationBusiness.Insert(user.ID, user.FullName, 0, "Admin", data.ID, "", notiMessage, notiMessage2, 3, url, user.Username, true);
                            result.IsError = false;
                            result.Message = "Yêu cầu của Quý khách đã được gửi!";
                            result.Data = true;
                            return result;
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                            result.Data = false;
                            return result;
                        }    
                    }
                    else
                    {

                        result.IsError = true;
                        result.Data = false;
                        result.Message = "Không tìm thấy thông tin khách hàng. Vui lòng thử lại sau!";
                        return result;
                    }    
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi tạo lệnh rút tiền: ", JsonConvert.SerializeObject(ex));
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                result.Data = false;
                return result;
            }
        }

        public static DataReturnModel<tbl_Withdraw> Approval(int ID, string userLogin)
        {
            DataReturnModel<tbl_Withdraw> result = new();
            try
            {
                using (var db = new nhanshiphangContext())
                {
                    var widthDraw = db.tbl_Withdraws.FirstOrDefault(x => x.ID == ID);
                    if (widthDraw != null)
                    {
                        if (widthDraw.Status == 2)
                        {
                            var modifiter = db.tbl_Accounts.FirstOrDefault(x => x.Username == widthDraw.ModifiedBy);
                            if (modifiter != null)
                                result.Message = string.Format("Yêu cầu đã được {0} duyệt trước đó!", modifiter.FullName);
                            else
                                result.Message = "Yêu cầu đã được duyệt!";
                            result.IsError = false;
                            result.Data = widthDraw;
                            return result;
                        }
                        var admin = db.tbl_Accounts.FirstOrDefault(x => x.Username == userLogin.ToLower());
                        var user = db.tbl_Accounts.FirstOrDefault(x => x.ID == widthDraw.UID);
                        if (user != null && admin != null)
                        {
                            string money = Converted.Double2Money(Converted.ToDouble(widthDraw.Amount));
                            string message = "";
                            string message2 = "";
                            if (widthDraw.Type == 1)
                            {
                                string moneyPrevious = user.Wallet;
                                string moneyLef = Converted.Double2Money(Converted.ToDouble(user.Wallet) + Converted.ToDouble(widthDraw.Amount));
                                user.Wallet = moneyLef;
                                message = string.Format("Tài khoản của Quý khách đã được <span class=\"text-nsh2\">+{0}</span>", money);
                                message2 = string.Format("Tài khoản của Quý khách đã được +{0}", money);
                                HistoryPayWallet.Insert(user.ID, user.Username, widthDraw.ID, widthDraw.Note, widthDraw.Amount, 2, 3, moneyPrevious, moneyLef, admin.Username);
                                NotificationBusiness.Insert(admin.ID, admin.Username, user.ID, user.Username, widthDraw.ID, "", message, message2, 2, "", admin.Username);
                            }
                            else if (widthDraw.Type == 2)
                            {
                                double wallet = Converted.ToDouble(user.Wallet) - Converted.ToDouble(widthDraw.Amount);
                                message = string.Format("Yêu cầu rút <span class=\"text-nsh1\">{0}</span> của Quý khách đã được duyệt.", money);
                                message2 = string.Format("Yêu cầu rút {0} của Quý khách đã được duyệt.", money);
                                var history = db.tbl_HistoryPayWallets.FirstOrDefault(x => x.OrderID == widthDraw.ID);
                                if (history != null)
                                {
                                    history.Status = 1;
                                    history.IsActive = 1;
                                    db.Update(history);
                                }
                                NotificationBusiness.Insert(admin.ID, admin.Username, user.ID, user.Username, widthDraw.ID, "", message, message2, 3, "", admin.Username);
                            }
                            widthDraw.Status = 2;
                            widthDraw.ModifiedBy = userLogin;
                            widthDraw.ModifiedDate = DateTime.Now;
                            db.Update(widthDraw);
                            user.ModifiedBy = admin.Username;
                            user.ModifiedDate = DateTime.Now;
                            db.Update(user);
                            int kq = db.SaveChanges();
                            if (kq > 0)
                            {
                                result.IsError = false;
                                result.Message = "Duyệt thành công!";
                                result.Data = widthDraw;
                                return result;
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                                return result;
                            }
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Không tìm thấy thông tin khách hàng. Vui lòng thử lại!";
                            return result;
                        }    
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Không tìm thấy thông tin giao dịch. Vui lòng thử lại!";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi duyệt lệnh nạp/rút tiền: ", JsonConvert.SerializeObject(ex));
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                return result;
            }
        }

        public static DataReturnModel<bool> Refuse(int ID, string createdBy)
        {
            var result = new DataReturnModel<bool>();
            try
            {
                using (var db = new nhanshiphangContext())
                {
                    var widthDraw = db.tbl_Withdraws.FirstOrDefault(x => x.ID == ID);
                    if (widthDraw != null)
                    {
                        if (widthDraw.Status == 3)
                        {
                            result.IsError = false;
                            result.Data = true;
                            var editer = db.tbl_Accounts.FirstOrDefault(x => x.Username == widthDraw.ModifiedBy);
                            if (editer != null)
                                result.Message = string.Format("Yêu cầu đã được {0} từ chối!", editer.FullName);
                            else
                                result.Message = "Yêu cầu đã được từ chối!";
                            return result;
                        }
                        var user = db.tbl_Accounts.FirstOrDefault(x => x.ID == widthDraw.UID);
                        var admin = db.tbl_Accounts.FirstOrDefault(x => x.Username == createdBy);
                        if (user != null && admin != null)
                        {
                            if (widthDraw.Type == 2)
                            {
                                var historyWallet = db.tbl_HistoryPayWallets.FirstOrDefault(x => x.OrderID == widthDraw.ID);
                                if (historyWallet != null)
                                {
                                    historyWallet.Status = 2;
                                    historyWallet.IsActive = 1;
                                    db.Update(historyWallet);
                                }
                                user.Wallet = Converted.Double2Money(Converted.ToDouble(user.Wallet) + Converted.ToDouble(widthDraw.Amount));
                                user.ModifiedBy = createdBy;
                                user.ModifiedDate = DateTime.Now;
                                db.Update(user);
                            }
                            widthDraw.Status = 3;
                            widthDraw.ModifiedBy = createdBy;
                            widthDraw.ModifiedDate = DateTime.Now;
                            db.Update(widthDraw);
                            int kq = db.SaveChanges();
                            if (kq > 0)
                            {
                                string money = Converted.Double2Money(Converted.ToDouble(widthDraw.Amount));
                                string message = "";
                                string message2 = "";
                                if (widthDraw.Type == 1)
                                {
                                    message = string.Format("Yêu cầu nạp <span class=\"text-nsh2\">{0}</span> của Quý khách đã bị từ chối.", money);
                                    message2 = string.Format("Yêu cầu nạp {0} của Quý khách đã bị từ chối.", money);
                                    NotificationBusiness.Insert(admin.ID, admin.Username, user.ID, user.Username, widthDraw.ID, "", message, message2, 2, "", admin.Username);
                                }
                                if (widthDraw.Type == 2)
                                {
                                    message = string.Format("Yêu cầu rút <span class=\"text-nsh1\">{0}</span> của Quý khách đã bị từ chối.", money);
                                    message2 = string.Format("Yêu cầu rút {0} của Quý khách đã bị từ chối.", money);
                                    NotificationBusiness.Insert(admin.ID, admin.Username, user.ID, user.Username, widthDraw.ID, "", message, message2, 3, "", admin.Username);
                                }
                                result.IsError = false;
                                result.Data = true;
                                result.Message = "Từ chối thành công!";
                                return result;
                            }
                            else
                            {
                                result.IsError = true;
                                result.Data = false;
                                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                                return result;
                            }
                        }
                        else
                        {
                            result.IsError = true;
                            result.Data = false;
                            result.Message = "Không tìm thấy thông tin khách hàng. Vui lòng thử lại sau!";
                            return result;
                        }    
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Không tìm thấy thông tin giao dịch. Vui lòng thử lại!";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi từ chối nạp/rút tiền: ", JsonConvert.SerializeObject(ex));
                result.IsError = true;
                result.Data = false;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                return result;
            }
        }

        public static DataReturnModel<bool> BuySearches(string userName,int amount, bool free, string createdBy = "")
        {
            DataReturnModel<bool> result = new DataReturnModel<bool>();
            try
            {
                if (amount < 1)
                {
                    result.IsError = true;
                    result.Message = "Vui lòng nhập số lượt quy đổi!";
                    return result;
                }
                using (var db = new nhanshiphangContext())
                {
                    var user = db.tbl_Accounts.FirstOrDefault(x => x.Username == userName.ToLower());

                    if (user != null)
                    {
                        if(free)
                        {
                           
                            tbl_Account? admin = db.tbl_Accounts.FirstOrDefault(x => x.Username == createdBy.ToLower());
                            if (admin != null)
                            {
                                user.AvailableSearch = user.AvailableSearch + amount;
                                user.ModifiedBy = admin.Username;
                                user.ModifiedDate = DateTime.Now;
                                db.Update(user);
                                int kq = db.SaveChanges();
                                if(kq > 0)
                                {
                                    PointsBusiness.Insert(user.ID, user.Username, "", $"Được tặng tại hệ thống tracking.nhanshiphang.vn", amount, 0, user.AvailableSearch.Value, createdBy);

                                    _ = Task.Run(async () =>
                                    {
                                        string message = $"Quý khách được tặng {amount} lượt tìm kiếm hàng trên Baidu.";
                                        await NotificationBusiness.Insert(admin.ID, admin.FullName, user.ID, user.FullName, 0, "", message, message, 6, "", createdBy);
                                    });
                                    result.IsError = false;
                                    result.Data = true;
                                    result.Message = "Đổi thành công!";
                                    return result;
                                }
                            }
                            result.IsError = true;
                            result.Message = "Hệ thống thực thi không thành công.Vui lòng thử lại sau!";
                            return result;
                        }
                        double money = amount * 500;
                        double wallet = Converted.ToDouble(user.Wallet) - money;
                        if (wallet < 0)
                        {
                            result.IsError = true;
                            result.Message = "Số dư ví không đủ!";
                            return result;
                        }
                        else
                        {
                            string moneyPrevious = Converted.String2Money(user.Wallet);
                            string moneyLeft = Converted.Double2Money(wallet);
                            user.Wallet = moneyLeft;
                            user.AvailableSearch = user.AvailableSearch + amount;
                            user.ModifiedBy = userName;
                            user.ModifiedDate = DateTime.Now;
                            if (!string.IsNullOrEmpty(createdBy))
                            {
                                tbl_Account? admin = db.tbl_Accounts.FirstOrDefault(x => x.Username == createdBy.ToLower());
                                if (admin != null)
                                {
                                    user.ModifiedBy = admin.Username;
                                    _ = Task.Run(async () =>
                                    {
                                        string message = $"Đổi {amount} lượt tìm kiếm hàng trên Baidu.";
                                        await NotificationBusiness.Insert(admin.ID,admin.FullName,user.ID,user.FullName,0,"",message, message, 6,"",createdBy);
                                    });
                                }
                            }
                            else
                                createdBy = userName;   
                            db.Update(user);
                            int kq = db.SaveChanges();
                            if (kq > 0)
                            {
                                string content = $"Đổi {amount} lượt tìm kiếm hàng trên Baidu.";
                                HistoryPayWallet.Insert(user.ID, user.Username, 0, content, Converted.Double2Money(money), 1, 6, moneyPrevious, moneyLeft, createdBy);
                                PointsBusiness.Insert(user.ID, user.Username, "", $"Mua điểm tại hệ thống tracking.nhanshiphang.vn", amount, 0, user.AvailableSearch.Value, createdBy);
                                result.IsError = false;
                                result.Data = true;
                                result.Message = "Đổi thành công!";
                                return result;
                            }
                            else
                            {
                                result.IsError = true;
                                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                                return result;
                            }
                        }
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Hệ thống không tìm thấy thống tin tài khoản!";
                        return result;
                    }
                };
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi đổi lượt tìm kiếm baidu: ", JsonConvert.SerializeObject(ex));
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                return result;
            }
        }
    }
}
