using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_TradeHistory
    {
        public int ID { get; set; }
        public int? UID { get; set; }
        public string? Username { get; set; }
        public string? Title { get; set; }
        public string? OrderCode { get; set; }
        public string? OrderPrice { get; set; }
        public string? AmountIn { get; set; }
        public string? AmountDebt { get; set; }
        /// <summary>
        /// 1. Tiền măt
        /// 2. Chuyển khoản
        /// 3. Chọn tài khoản thanh toán
        /// 4. Khác
        /// </summary>
        public int? PaymentMethod { get; set; }
        public string? Note { get; set; }
        /// <summary>
        /// Mã thanh toán
        /// </summary>
        public string? PayCode { get; set; }
        /// <summary>
        /// 1. Nạp tiền
        /// 
        /// </summary>
        public int? PayType { get; set; }
        /// <summary>
        /// 1. Chưa duyệt
        /// 2. Đã duyệt
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 1. Admin Tạo
        /// 2. User Tạo
        /// 3. Đơn hàng đã duyệt tạo
        /// </summary>
        public int? Type { get; set; }
        /// <summary>
        /// 1. Tiền hóa đơn
        /// 2. Nạp tiền
        /// 3. Tiền khiếu nại
        /// 4. Rút tiền
        /// </summary>
        public int? TradeType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
