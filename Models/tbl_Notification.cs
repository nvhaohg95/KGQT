using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Notification
    {
        public int ID { get; set; }
        public int? SenderID { get; set; }
        public string? SenderUsername { get; set; }
        public int? ReceivedID { get; set; }
        public string? ReceivedUsername { get; set; }
        public int? OrderID { get; set; }
        public string? OrderCode { get; set; }
        public string? Message { get; set; }
        /// <summary>
        /// 0. chưa đọc
        /// 1. đã xem
        /// </summary>
        public int? Status { get; set; }
        public bool? IsForAdmin { get; set; }
        /// <summary>
        /// 1. Đơn hàng
        /// 2. Nạp tiền
        /// 3. Rút tiền
        /// 4. Yêu cầu giao
        /// 5. Khiếu nại
        /// 6. Nạp tiền tại kho
        /// 7. Rút tiền tại kho
        /// 8. Truy thu
        /// 9. Phí giao hàng
        /// </summary>
        public int? NotifType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        /// <summary>
        /// ID người hỗ trợ
        /// </summary>
        public int? SuportID { get; set; }
        public bool? IsWait { get; set; }
        public int? ComplainID { get; set; }
    }
}
