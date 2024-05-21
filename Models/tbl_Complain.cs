using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Complain
    {
        public int ID { get; set; }
        public int? UID { get; set; }
        public string? Title { get; set; }
        /// <summary>
        /// 0. Khiếu nại hệ thống; 1. Khiếu nại kiện;
        /// 2. Khiếu nại đơn; 3. Khiếu nại nạp tiền
        /// </summary>
        public int? Type { get; set; }
        public string? TransId { get; set; }
        public string? Context { get; set; }
        public string? IMG { get; set; }
        /// <summary>
        /// 1. Chờ duyệt
        /// 2. Đã duyệt
        /// 3. Hủy
        /// </summary>
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
