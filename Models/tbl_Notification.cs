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
        public string? Message2 { get; set; }
        /// <summary>
        /// 0. Chưa xem
        /// 1. Đã xem
        /// </summary>
        public int? Status { get; set; }
        public bool? IsForAdmin { get; set; }
        /// <summary>
        /// 1. Đơn hàng
        /// 2. Nạp tiền
        /// 3. Rút tiền
        /// 4. Yêu cầu giao
        /// 5. Khiếu nại
        /// 6. Đổi lượt tìm kiếm baidu
        /// 10. Thông báo chung
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
        public string? Url { get; set; }
    }
}
