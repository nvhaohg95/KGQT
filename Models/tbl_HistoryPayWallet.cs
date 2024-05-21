using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_HistoryPayWallet
    {
        public int ID { get; set; }
        public int UID { get; set; }
        public string Username { get; set; } = null!;
        public int OrderID { get; set; }
        public string Amount { get; set; } = null!;
        public string? HContent { get; set; }
        public string MoneyLeft { get; set; } = null!;
        public string? MoneyPrevious { get; set; }
        /// <summary>
        /// 1: trừ
        /// 2: cộng
        /// 
        /// </summary>
        public int Type { get; set; }
        public int? Status { get; set; }
        public int? IsActive { get; set; }
        /// <summary>
        /// 1: Thanh toán đơn hàng, 
        /// 2: Nhận lại tiền hang, 3: Admin nạp tiền
        /// , 4: Rút tiền
        /// 5: Hủy rút tiền, 6:Nạp tiền tại kho
        /// 7.Rút tiền tại kho
        /// 
        /// </summary>
        public int TradeType { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}
