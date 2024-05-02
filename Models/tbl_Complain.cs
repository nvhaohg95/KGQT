using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KGQT.Models
{
    public partial class tbl_Complain
    {
        public int ID { get; set; }
        public int? UID { get; set; }
        /// <summary>
        /// 0. Khiếu nại thiếu hàng
        /// 1. Khiếu nại sai mẫu (bồi thường)
        /// 2. Khiếu nại sai mẫu (trả hàng)
        /// </summary>
        public int? Type { get; set; }

        [MaxLength(50)]
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
        public string? Title { get; set; }

        //public int? OrderShopID { get; set; }
        //public string? OrderShopCode { get; set; }
        //public string? OrderCodeCompare { get; set; }
        //public string? Amount { get; set; }
        //public string? ComplainText { get; set; }
        //public int? SupportID { get; set; }
        //public string? AmountTruyThu { get; set; }
        //public bool? IsTruyThu { get; set; }

    }
}
