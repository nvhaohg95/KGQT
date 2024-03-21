using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Withdraw
    {
        public int ID { get; set; }
        public int? UID { get; set; }
        public string? Username { get; set; }
        public string? Fullname { get; set; }
        public string? Amount { get; set; }
        public string? BankName { get; set; }
        /// <summary>
        /// 1. Tiền mặt
        /// 2. Chuyển khoản
        /// 
        /// </summary>
        public int? PaymentMethod { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public string? Note { get; set; }
        /// <summary>
        /// 1: Đang chờ
        /// 2: Thành công
        /// 3: Hủy
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 1. Nạp tiền
        /// 2. Rút tiền
        /// 3. Truy thu
        /// 4. Truy thu khác
        /// 5. Chi
        /// 6. Nạp tiền tại kho
        /// 7. Rút tiền tại kho
        /// 8. Chi công ty
        /// 9. Thu công ty
        /// </summary>
        public int? Type { get; set; }
        public DateTime? DateSend { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? CustomerFile { get; set; }
        public string? AcceptBy { get; set; }
        public DateTime? AcceptDate { get; set; }
        /// <summary>
        /// 1. Phí giao hàng
        /// 2. Khác
        /// </summary>
        public int? TradeType { get; set; }
        public string? DiscountCommission { get; set; }
        public double? WithdrawReal { get; set; }
        public double? Fee { get; set; }
        public string? tid { get; set; }
        public int? CassoID { get; set; }
    }
}
