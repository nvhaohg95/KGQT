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
    }
}
