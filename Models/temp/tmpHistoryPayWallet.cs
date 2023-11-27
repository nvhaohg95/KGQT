namespace KGQT.Models.temp
{
    public class tmpHistoryPayWallet : tbl_HistoryPayWallet
    {
        public AccountInfo Customer { get; set; }
        public string CreatedName { get; set; }
        public tmpHistoryPayWallet(tbl_HistoryPayWallet data)
        {
            ID = data.ID;
            UID = data.UID;
            Username = data.Username;
            OrderID = data.OrderID;
            Amount = data.Amount;
            HContent = data.HContent;
            MoneyLeft = data.MoneyLeft;
            Type = data.Type;
            TradeType = data.TradeType;
            Note = data.Note;
            CreatedDate = data.CreatedDate;
            CreatedBy = data.CreatedBy;
        }

    }
}
