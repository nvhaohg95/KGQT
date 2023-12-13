using KGQT.Models;
using KGQT.Models.temp;

namespace KGQT.Business
{
    public class WithDrawBusiness
    {
        #region Get page
        public static object[] GetList(int status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            var lstData = new List<tbl_Withdraw>();
            int count = 0;
            int totalPage = 0;
            using (var db = new nhanshiphangContext())
            {
                var query = db.tbl_Withdraws.AsQueryable();
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
        public static bool Insert(tbl_Withdraw data,string createdBy)
        {
            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.Username);
                if (acc != null)
                {
                    var user = AccountBusiness.GetFullInfo(null, 0, acc.Username);
                    var admin = AccountBusiness.GetFullInfo(null, 0, createdBy);
                    db.Add(data);
                    int kq1 = db.SaveChanges();
                    if(kq1 > 0)
                    {
                        string notiMessage = "";
                        if (data.Status == 2)
                        {
                            var moneyLeft = acc.Wallet != null ? acc.Wallet : 0;
                            acc.Wallet = moneyLeft + data.Amount;
                            acc.ModifiedBy = createdBy;
                            acc.ModifiedDate = DateTime.Now;
                            db.Update(acc);
                            db.SaveChanges();
                            notiMessage = string.Format("Nạp tiền thành công. Tài khoản của bạn được cộng thêm <span class=\"text-success\">+{0}</span>", data.Amount);
                            TransactionBusiness.Insert(acc.ID, data.Amount, 1, data.Type, createdBy);
                            HistoryPayWallet.Insert(acc.ID, acc.Username, data.ID, data.Note, data.Amount.Value, 2, 3, moneyLeft.Value, createdBy);
                            NotificationBusiness.Insert(admin.ID, string.Concat(admin.FirstName," ", admin.LastName), user.ID, string.Concat(user.FirstName," ",user.LastName), data.ID, "", notiMessage, 2, admin.Username);

                        }
                        else
                        {
                            notiMessage = string.Format("Yêu cầu nạp tiền. Khách hàng <span class=\"fw-bold\">{0}</span> yêu cầu nạp <span class=\"text-success\">{1}</span> vào tài khoản.", string.Concat(user.FirstName, " ", user.LastName), data.Amount);
                            NotificationBusiness.Insert(user.ID, string.Concat(user.FirstName, " ", user.LastName), 0, "Admin", data.ID, "", notiMessage, 2, acc.Username, true);
                        }
                        return true;
                    }
                }
                return false;
            }
        }
        #endregion

        #region Duyệt lệnh nạp tiền
        public static DataReturnModel<tbl_Withdraw> Approval(int ID, string userName)
        {
            var result = new DataReturnModel<tbl_Withdraw>();
            using (var db = new nhanshiphangContext())
            {
                var admin = db.tbl_Accounts.FirstOrDefault(x => x.Username == userName);
                var data = db.tbl_Withdraws.FirstOrDefault(x => x.ID == ID);
                if(admin != null && data != null)
                {
                    data.Status = 2;
                    data.ModifiedBy = admin.Username;
                    data.ModifiedDate = DateTime.Now;
                    db.Update(data);
                    var user = db.tbl_Accounts.FirstOrDefault(x => x.Username == userName);
                    var moneyLeft = user.Wallet != null ? user.Wallet : 0;
                    if (user != null)
                    {
                        user.Wallet = moneyLeft + data.Amount;
                        user.ModifiedBy = admin.Username;
                        user.ModifiedDate = DateTime.Now;
                        db.Update(user);
                        int isSave = db.SaveChanges();
                        if (isSave > 0)
                        {
                            TransactionBusiness.Insert(user.ID, data.Amount, 1, data.Type, admin.Username);
                            HistoryPayWallet.Insert(user.ID, user.Username, data.ID,data.Note, data.Amount.Value, 2, 3, moneyLeft.Value, userName);
                            NotificationBusiness.Insert(admin.ID, admin.Username, user.ID, user.Username, data.ID, "", string.Format("Nạp tiền thành công. Tài khoản của bạn được cộng thêm <span class=\"text-success\">+{0}</span>", data.Amount), 2, admin.Username);
                            result.IsError = false;
                            result.Message = "Duyệt thành công!";
                            result.Data = data;
                            return result;
                        }
                    }
                       
                }    
            }
            result.IsError = true;
            result.Message = "Hệ thống thực thi không thành công!";
            return result;
        }
        #endregion
    }
}
