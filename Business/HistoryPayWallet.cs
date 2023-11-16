using KGQT.Business.Base;
using KGQT.Models;

namespace KGQT.Business
{
    public static class HistoryPayWallet
    {
        #region CRUD
        public static bool Insert(int uid,string username,int orderId,double amount,int type, int tradeType, double moneyleft)
        {
            tbl_HistoryPayWallet pay = new tbl_HistoryPayWallet();
            pay.UID = uid;
            pay.Username = username;
            pay.OrderID = orderId;
            pay.Amount = amount;
            pay.Type = type;
            pay.TradeType = tradeType;
            pay.MoneyLeft = moneyleft;
            return BusinessBase.Add(pay);
        }
        #endregion


        #region Get Page
        public static object[] GetPage(string userName, int orderID, int tradeType, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tbl_HistoryPayWallet>();
                int totalRecod = 0;
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
                totalRecod = query.Count();
                if(totalRecod > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)totalRecod / pageSize));
                    lstData = query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }
                return new object[] {lstData,totalRecod,totalPage };
            }
        }
        #endregion
    }
}
