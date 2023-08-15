using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_HistoryPayWallet
    {
        public int ID { get; set; }
        public int? UID { get; set; }
        public string? UserName { get; set; }
        public int? MainOrderID { get; set; }
        public string? OrderCode { get; set; }
        public double? Amount { get; set; }
        public string? HContent { get; set; }
        public double? MoneyLeft { get; set; }
        /// <summary>
        /// 1: trừ
        /// 2: cộng
        /// 
        /// </summary>
        public int? Type { get; set; }
        /// <summary>
        /// 1: Đặt cọc
        /// 2: Nhận lại tiền đặt cọc
        /// 3: Thanh toán hóa đơn
        /// 4: Admin nạp tiền
        /// 5: Rút tiền
        /// 6: Hủy rút tiền
        /// 7. Nhận tiền khiếu nại đơn hàng
        /// 8. Trừ tiền lưu kho
        /// 9. Truy thu
        /// 11.Nạp tiền tại kho
        /// 12.Rút tiền tại kho
        /// 
        /// </summary>
        public int? TradeType { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}
