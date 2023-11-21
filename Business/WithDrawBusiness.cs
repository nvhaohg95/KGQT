using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.DiaSymReader;
using Newtonsoft.Json;

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
                    lstData = query.Skip((page - 1) * pageSize).Take(pageSize).OrderByDescending(x => x.CreatedDate).ToList();
                }
            }
            return new object[] { lstData, count, totalPage };
        }

        #endregion

        #region Add
        public static bool Insert(tbl_Withdraw data,string userLogin)
        {
            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.Username);
                if (acc != null)
                {
                    db.Add(data);
                    int kq1 = db.SaveChanges();
                    if(kq1 > 0)
                    {
                        if (data.Status == 2)
                        {
                            var moneyLeft = acc.Wallet != null ? acc.Wallet : 0;
                            acc.Wallet = moneyLeft + data.Amount;
                            acc.ModifiedBy = userLogin;
                            acc.ModifiedDate = DateTime.Now;
                            db.Update(acc);
                            db.SaveChanges();
                            TransactionBusiness.Insert(acc.ID, data.Amount, 1, data.Type, userLogin);
                            HistoryPayWallet.Insert(acc.ID, acc.Username, data.ID, data.Note, data.Amount.Value, 2, 3, moneyLeft.Value, userLogin);
                        }
                        return true;
                    }
                }
                return false;
            }
        }
        #endregion

        #region Duyệt lệnh nạp tiền
        public static DataReturnModel Approval(int ID, string userName)
        {
            var result = new DataReturnModel();
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
