using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;

namespace KGQT.Business
{
    public static class TransactionBusiness
    {
        #region Lấy danh sách giao dịch
        //public static object[] GetListTransaction(int status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        //{
        //    var lstData = new List<tbl_Transaction>();
        //    int count = 0;
        //    int totalPage = 0;
        //    using (var db = new nhanshiphangContext())
        //    {
        //        var query = db.tbl_Transactions.AsQueryable();
        //        if (status != 0)
        //            query = query.Where(x => x.Status == status);
        //        if (fromDate != null)
        //            query = query.Where(x => x.CreatedDate >= fromDate);
        //        if (fromDate != null)
        //            query = query.Where(x => x.CreatedDate <= toDate);
        //        count = query.Count();
        //        if (count > 0)
        //        {
        //            totalPage = Convert.ToInt32(Math.Ceiling((decimal)count / pageSize));
        //            lstData = query.Skip((page - 1) * pageSize).Take(pageSize).OrderByDescending(x => x.CreatedDate).ToList();
        //        }
        //    }
        //    return new object[] { lstData, count, totalPage };
        //}
        #endregion

        #region Duyệt giao dịch nạp tiền
        public static DataReturnModel<object> ApprovalRecharge(int ID, string createdBy)
        {
            var result = new DataReturnModel<object>();
            using (var db = new nhanshiphangContext())
            {
                var withDraw = db.tbl_Withdraws.FirstOrDefault(x => x.ID == ID);
                if (withDraw != null)
                {
                    withDraw.AcceptBy = createdBy;
                    withDraw.AcceptDate = DateTime.Now;
                    var user = db.tbl_Accounts.FirstOrDefault(x => x.ID == withDraw.UID);
                    if (user != null)
                    {
                        var moneyLeft = user.Wallet != null ? user.Wallet : 0;
                        user.Wallet = moneyLeft + withDraw.Amount;
                        db.Update(user);
                        db.Update(withDraw);
                        var isSave = db.SaveChanges();
                        if (isSave == 3)
                        {

                            result.IsError = false;
                            result.Message = "Duyệt thành công";
                            #region Logs
                            HistoryPayWallet.Insert(user.ID, user.Username, withDraw.ID, withDraw.Note, withDraw.Amount.Value, 1, 1, moneyLeft.Value, createdBy);
                            #endregion
                            return result;
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại";
                        }
                    }
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Không tìm thấy yêu cầu!";
                }
            }
            return result;
        }
        #endregion

    }
}
