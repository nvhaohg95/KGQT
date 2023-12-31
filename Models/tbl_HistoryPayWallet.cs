﻿using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_HistoryPayWallet
    {
        public int ID { get; set; }
        public int UID { get; set; }
        public string Username { get; set; } = null!;
        public int OrderID { get; set; }
        public double Amount { get; set; }
        public string? HContent { get; set; }
        public double MoneyLeft { get; set; }
        /// <summary>
        /// 1: trừ
        /// 2: cộng
        /// 
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 1: Thanh toán đơn hàng, 
        /// 2: Nhận lại tiền hang, 3: Nạp tiền
        /// , 4: Rút tiền
        /// 5: Hủy rút tiền, 6:Nạp tiền tại kho
        /// 7.Rút tiền tại kho
        /// 
        /// </summary>
        public int TradeType { get; set; }
        public string? Note { get; set; }
        /// <summary>
        /// 0:  chưa duyệt
        /// 1 : đã duyệt
        /// 2 : từ chối ( dùng cho nạp/rút tiền )
        /// </summary>
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}
