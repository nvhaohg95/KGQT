using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;

namespace KGQT.Business
{
    public static class TransactionBusiness
    {
        #region Lấy danh sách giao dịch nạp tiền
        public static object[] GetListTransaction(int status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            var lstData = new List<tbl_Transaction>();
            int count = 0;
            int totalPage = 0;
            using (var db = new nhanshiphangContext())
            {
                var query = db.tbl_Transactions.AsQueryable();
                if (status != 0)
                    query = query.Where(x => x.Status == status);
                if (fromDate != null)
                    query = query.Where(x => x.CreatedDate >= fromDate);
                if (fromDate != null)
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

        #region Duyệt giao dịch nạp tiền
        public static DataReturnModel ApprovalRecharge(int ID, string createdBy)
        {
            var result = new DataReturnModel();
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
                        var transaction = new tbl_Transaction()
                        {
                            UIDReceive = user.ID,
                            Amount = withDraw.Amount,
                            Type = 2,
                            CreatedBy = createdBy,
                            CreatedDate = DateTime.Now
                        };
                        switch (withDraw.Type)
                        {
                            case 1:
                            case 6:
                                transaction.Type = 1;
                                break;
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 7:
                            case 8:
                            case 9:
                                transaction.Type = 2;
                                break;
                        }
                        db.Add(transaction);
                        db.Update(user);
                        db.Update(withDraw);
                        var isSave = db.SaveChanges();
                        if (isSave == 3)
                        {

                            result.IsError = false;
                            result.Message = "Duyệt thành công.";
                            #region Logs
                            HistoryPayWallet.Insert(user.ID, user.Username, withDraw.ID, withDraw.Note, withDraw.Amount.Value, 1, 1, moneyLeft.Value, createdBy);
                            #endregion
                            return result;
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Hệ thống thực thi không thành công!";
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


        #region Add
        public static bool Insert(int? UID, double? amount, int? inOut, int? withDrawType, string createdBy)
        {
            using (var db = new nhanshiphangContext())
            {
                var transaction = new tbl_Transaction()
                {
                    UIDReceive = UID,
                    Amount = amount,
                    INOUT = inOut,
                    Type = 2,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                };
                switch (withDrawType)
                {
                    case 1:
                    case 6:
                        transaction.Type = 1;
                        break;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 7:
                    case 8:
                    case 9:
                        transaction.Type = 2;
                        break;
                }
                db.Add(transaction);
                var kq = db.SaveChanges();
                if (kq > 0)
                    return true;
            }
            return false;
        }
        #endregion
    }
}
