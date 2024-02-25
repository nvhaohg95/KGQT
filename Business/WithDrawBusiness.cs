using KGQT.Models;
using KGQT.Models.temp;
using System;

namespace KGQT.Business
{
    public class WithDrawBusiness
    {
        #region Get page
        public static object[] GetList(int ID,int type,int status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            var lstData = new List<tbl_Withdraw>();
            int count = 0;
            int totalPage = 0;
            using (var db = new nhanshiphangContext())
            {
                var query = db.tbl_Withdraws.AsQueryable();
                if(ID > 0)
                    query = query.Where(x => x.ID == ID);
                if (status != 0)
                    query = query.Where(x => x.Type == type);
                if (status != 0)
                    query = query.Where(x => x.Status == status);
                if (fromDate != null)
                    query = query.Where(x => x.CreatedDate >= fromDate);
                if (toDate != null)
                    query = query.Where(x => x.CreatedDate <= toDate);
                count = query.Count();
                if (count > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)count / pageSize));
                    lstData = query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }
            }
            return new object[] { lstData, count, totalPage };
        }

        #endregion

        #region Add
        public static DataReturnModel<bool> Insert(tbl_Withdraw data,string createdBy)
        {
            DataReturnModel<bool> result = new DataReturnModel<bool>();
            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.Username);
                if (acc != null)
                {
                    if (data.Type == 2 && acc.Wallet < data.Amount)
                    {
                        result.IsError = true;
                        result.Message = "Số dư trong ví của khách không đủ.";
                        result.Data = false;
                        return result;
                    }
                    var user = AccountBusiness.GetInfo(-1, acc.Username);
                    var admin = AccountBusiness.GetInfo(-1, createdBy);
                    db.Add(data);
                    int kq1 = db.SaveChanges();
                    if(kq1 > 0)
                    {
                        string notiMessage = "";
                        if (data.Status == 2)
                        {
                            string strHTML = string.Format("{0:N0}đ", data.Amount).Replace(",", ".");
                            if (data.Type == 1)
                            {
                                var moneyLeft = acc.Wallet != null ? acc.Wallet : 0;
                                acc.Wallet = moneyLeft + data.Amount;
                                acc.ModifiedBy = createdBy;
                                acc.ModifiedDate = DateTime.Now;
                                db.Update(acc);
                                db.SaveChanges();
                                notiMessage = string.Format("Tài khoản của bạn đã được <span class=\"text-success\">+{0}</span>", strHTML);
                                HistoryPayWallet.Insert(acc.ID, acc.Username, data.ID, data.Note, data.Amount.Value, 2, 3, moneyLeft.Value, createdBy);
                                NotificationBusiness.Insert(admin.ID, admin.FullName, user.ID, user.FullName, data.ID, "", notiMessage, 2,"" ,admin.Username);
                            }
                            else if(data.Type == 2)
                            {
                                var moneyLeft = acc.Wallet != null ? acc.Wallet : 0;
                                acc.Wallet = moneyLeft - data.Amount;
                                acc.ModifiedBy = createdBy;
                                acc.ModifiedDate = DateTime.Now;
                                db.Update(acc);
                                db.SaveChanges();
                                notiMessage = string.Format("Yêu cầu rút <span class=\"text-danger\">{0}</span> của bạn đã được duyệt.", strHTML);
                                HistoryPayWallet.Insert(acc.ID, acc.Username, data.ID, data.Note, data.Amount.Value, 1, 4, moneyLeft.Value, createdBy);
                                NotificationBusiness.Insert(acc.ID, acc.FullName, acc.ID, acc.FullName, data.ID, "", notiMessage, 3, "", acc.Username);
                            }    
                            result.IsError = false;
                            result.Message = "Tạo thành công!";
                            result.Data = true;
                        }
                        else
                        {
                            string url = "/Admin/Withdraw/Index?ID=" + data.ID;
                            string strHTML = string.Format("{0:N0}đ", data.Amount).Replace(",", ".");
                            notiMessage = string.Format("Khách hàng <span class=\"fw-bold\">{0}</span> yêu cầu nạp <span class=\"text-success\">{1}</span> vào tài khoản.", user.FullName, strHTML);
                            NotificationBusiness.Insert(user.ID, user.FullName, 0, "Admin", data.ID, "", notiMessage, 2, url ,acc.Username, true);
                            result.IsError = false;
                            result.Message = "Yêu cầu của bạn đã được gửi!";
                            result.Data = true;
                        }
                        return result;
                    }
                }
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                result.Data = false;
                return result;
            }
        }
        // rút tiền
        public static DataReturnModel<bool> Insert2(tbl_Withdraw data, string createdBy)
        {
            DataReturnModel<bool> result = new DataReturnModel<bool>();
            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.Username);
                if (acc != null )
                {
                    if(acc.Wallet < data.Amount)
                    {
                        result.IsError = true;
                        result.Message = "Số dư trong ví của bạn không đủ.";
                        result.Data = false;
                        return result;
                    }
                    var admin = AccountBusiness.GetInfo(-1, createdBy);
                    var moneyLeft = acc.Wallet != null ? acc.Wallet : 0;
                    acc.Wallet = moneyLeft - data.Amount;
                    acc.ModifiedBy = createdBy;
                    acc.ModifiedDate = DateTime.Now;
                    db.Update(acc);
                    db.Add(data);
                    int isSave = db.SaveChanges();
                    if (isSave > 0)
                    {
                        string url = "/Admin/Refuse/Index?ID=" + data.ID;
                        string strHTML = string.Format("{0:N0}đ", data.Amount).Replace(",", ".");
                        string notiMessage = string.Format("Khách hàng <span class=\"fw-bold\">{0}</span> yêu cầu rút <span class=\"text-danger\">{1}</span>.", acc.FullName, strHTML); ;
                        HistoryPayWallet.Insert(acc.ID, acc.Username, data.ID, data.Note, data.Amount.Value, 1, 4, moneyLeft.Value, createdBy,0);
                        NotificationBusiness.Insert(acc.ID, acc.FullName, 0, "Admin", data.ID, "", notiMessage, 3, url, acc.Username, true);
                        result.IsError = false;
                        result.Message = "Yêu cầu của bạn đã được gửi!";
                        result.Data = true;
                        return result;
                    }
                }
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                result.Data = false;
                return result;
            }
        }
        #endregion

        #region Duyệt yêu cầu nạp/rút tiền
        public static DataReturnModel<tbl_Withdraw> Approval(int ID, string userName)
        {
            var result = new DataReturnModel<tbl_Withdraw>();
            using (var db = new nhanshiphangContext())
            {
                var widthDraw = db.tbl_Withdraws.FirstOrDefault(x => x.ID == ID);
                if(widthDraw != null)
                {
                    if(widthDraw.Status == 2)
                    {
                        var editer = db.tbl_Accounts.FirstOrDefault(x => x.Username == widthDraw.ModifiedBy);
                        if(editer != null)
                            result.Message = string.Format("Yêu cầu đã được {0} duyệt trước đó!",editer.FullName);
                        else
                            result.Message = "Yêu cầu đã được duyệt!";
                        result.IsError = false;
                        result.Data = widthDraw;
                        return result;
                    }
                    widthDraw.Status = 2;
                    widthDraw.ModifiedBy = userName;
                    widthDraw.ModifiedDate = DateTime.Now;
                    db.Update(widthDraw);
                    var admin = db.tbl_Accounts.FirstOrDefault(x => x.Username == userName);
                    var user = db.tbl_Accounts.FirstOrDefault(x => x.ID == widthDraw.UID);
                    if (user != null && admin != null)
                    {
                        string strHTML = string.Format("{0:N0}đ", widthDraw.Amount).Replace(",", ".");
                        string message = "";
                        if (widthDraw.Type == 1) //nạp tiền
                        {
                            var moneyLeft = user.Wallet != null ? user.Wallet : 0;
                            user.Wallet = moneyLeft + widthDraw.Amount;
                            message = string.Format("Tài khoản của bạn đã được <span class=\"text-success\">+{0}</span>", strHTML);
                            HistoryPayWallet.Insert(user.ID, user.Username, widthDraw.ID, widthDraw.Note, widthDraw.Amount.Value, 2, 3, moneyLeft.Value, userName);
                            NotificationBusiness.Insert(admin.ID, admin.Username, user.ID, user.Username, widthDraw.ID, "", message, 2, "", admin.Username);
                        }
                        else if (widthDraw.Type == 2)//rút tiền
                        {
                            message = string.Format("Yêu cầu rút <span class=\"text-danger\">{0}</span> của bạn đã được duyệt.", strHTML);
                            //HistoryPayWallet.Insert(user.ID, user.Username, data.ID, data.Note, data.Amount.Value, 1, 4, moneyLeft.Value, userName);
                            var history = db.tbl_HistoryPayWallets.FirstOrDefault(x => x.OrderID == widthDraw.ID);
                            if(history != null)
                            {
                                history.Status = 1;
                                db.Update(history);
                            }
                            NotificationBusiness.Insert(admin.ID, admin.Username, user.ID, user.Username, widthDraw.ID, "", message, 3, "", admin.Username);
                        }
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
                    }
                }    
            }
            result.IsError = true;
            result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
            return result;
        }
        #endregion

        #region Từ chối yêu cầu nạp/rút tiền
        public static DataReturnModel<bool> Refuse(int ID, string userName)
        {
            var result = new DataReturnModel<bool>();
            using (var db = new nhanshiphangContext())
            {
                var widthDraw = db.tbl_Withdraws.FirstOrDefault(x => x.ID == ID);
                if(widthDraw != null)
                {
                    if(widthDraw.Status == 3)
                    {
                        var editer = db.tbl_Accounts.FirstOrDefault(x => x.Username == widthDraw.ModifiedBy);
                        if (editer != null)
                            result.Message = string.Format("Yêu cầu đã được {0} từ chối trước đó!", editer.FullName);
                        else
                            result.Message = "Yêu cầu đã được từ chối!";
                        result.IsError = false;
                        result.Data = true;
                        return result;
                    }    
                    widthDraw.Status = 3;
                    widthDraw.ModifiedBy = userName;
                    widthDraw.ModifiedDate = DateTime.Now;
                    var user = db.tbl_Accounts.FirstOrDefault(x => x.ID == widthDraw.UID);
                    var admin = db.tbl_Accounts.FirstOrDefault(x => x.Username == userName);
                    if (user != null && admin != null)
                    {
                        if(widthDraw.Type == 2)
                        {
                            user.Wallet = user.Wallet + widthDraw.Amount;
                            user.ModifiedBy = userName;
                            user.ModifiedDate = DateTime.Now;
                            var historyWallet = db.tbl_HistoryPayWallets.FirstOrDefault(x => x.OrderID == widthDraw.ID);
                            historyWallet.Status = 2;
                            db.Update(historyWallet);
                            db.Update(user);
                        }
                        db.Update(widthDraw);
                        int kq = db.SaveChanges();
                        if(kq > 0)
                        {
                            string money = string.Format("{0:N0}đ", widthDraw.Amount).Replace(",", ".");
                            var message = "";
                            if(widthDraw.Type == 1)
                            {
                                message = string.Format("Yêu cầu nạp <span class=\"text-success\">{0}</span> của bạn đã bị từ chối.", money);
                                NotificationBusiness.Insert(admin.ID, admin.Username, user.ID, user.Username, widthDraw.ID, "", message, 2, "", admin.Username);
                            }
                            if (widthDraw.Type == 2)
                            {
                                message = string.Format("Yêu cầu rút <span class=\"text-danger\">{0}</span> của bạn đã bị từ chối.", money);
                                NotificationBusiness.Insert(admin.ID, admin.Username, user.ID, user.Username, widthDraw.ID, "", message, 3, "", admin.Username);
                            }    
                            result.IsError = false;
                            result.Data = true;
                            result.Message = "Từ chối thành công!";
                            return result;
                        }    
                    }    
                }    
            }
            result.IsError = true;
            result.Data = false;
            result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
            return result;
        }
        #endregion
    }
}
