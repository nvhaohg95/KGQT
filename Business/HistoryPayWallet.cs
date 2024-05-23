using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Serilog;
using ILogger = Serilog.ILogger;
namespace KGQT.Business
{
    public static class HistoryPayWallet
    {
        private static readonly ILogger _log = Log.ForContext(typeof(AccountBusiness));
        public static object[] GetPage(string userName, int orderID, int tradeType, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            using (var db = new nhanshiphangContext())
            {
                List<tbl_HistoryPayWallet> datas = new();
                int total = 0;
                int totalPage = 0;
                var query = db.tbl_HistoryPayWallets.Where(x => x.Username == userName);
                if (orderID != 0)
                    query = query.Where(x => x.OrderID == orderID);
                if (tradeType != 0)
                    query = query.Where(x => x.TradeType == tradeType);
                if (fromDate != null)
                    query = query.Where(x => x.CreatedDate >= fromDate);
                if (toDate != null)
                    query = query.Where(x => x.CreatedDate <= toDate);
                total = query.Count();
                if(total > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                    datas = query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }
                return new object[] {datas ,total, page };
            }
        }

        public static bool Insert(int uid, string username, int orderId, string content, string amount, int type, int tradeType, string moneyPrevious, string moneyleft, string createdBy, int status = 1, int isActive = 1)
        {
            tbl_HistoryPayWallet pay = new tbl_HistoryPayWallet();
            pay.UID = uid;
            pay.Username = username;
            pay.OrderID = orderId;
            pay.HContent = content;
            pay.Type = type;
            pay.TradeType = tradeType;
            pay.Amount = Converted.String2Money(amount);
            pay.MoneyPrevious = Converted.String2Money(moneyPrevious);
            pay.MoneyLeft = Converted.String2Money(moneyleft);
            pay.CreatedDate = DateTime.Now;
            pay.CreatedBy = createdBy;
            pay.Status = status;
            pay.IsActive = isActive;
            return BusinessBase.Add(pay);
        }

        public static tmpHistoryPayWallet? GetByID(int id)
        {
            tmpHistoryPayWallet? data = null;
            using (var db = new nhanshiphangContext())
            {
                var history = db.tbl_HistoryPayWallets.FirstOrDefault(x => x.ID == id);
                if (history != null)
                {
                    data = new tmpHistoryPayWallet(history);
                    var user = db.tbl_Accounts.FirstOrDefault(x => x.ID == history.UID);
                    if (user != null)
                    {
                        data.Customer = new AccountInfo();
                        data.Customer.ID = user.ID;
                        data.Customer.FullName = user.FullName;
                        data.Customer.Email = user.Email;
                        data.Customer.Phone = user.Phone;
                    }
                }
            }
            return data;
        }
    }
}
