using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Transaction
    {
        public int ID { get; set; }
        public int? UIDReceive { get; set; }
        public double? Amount { get; set; }
        /// <summary>
        /// 1. Tiền vào
        /// 2. Tiền ra
        /// </summary>
        public int? INOUT { get; set; }
        /// <summary>
        /// 1. Chờ duyệt
        /// 2. Đã duyệt
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 1. User gửi yêu cầu
        /// 2. Admin tạo
        /// </summary>
        public int? Type { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
