using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_HistoryExport
    {
        public int ID { get; set; }
        public string? OrderShopCode { get; set; }
        public int? OrderShopID { get; set; }
        public int? UID { get; set; }
        public string? Usename { get; set; }
        public int? CustomerType { get; set; }
        /// <summary>
        /// 1. Chờ xử lý
        /// 2. Đã xử lý
        /// 
        /// </summary>
        public int? Status { get; set; }
        public double? TotalPrice { get; set; }
        public double? TotalWeight { get; set; }
        public DateTime? DateInSG { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? PackageOrderHangCode { get; set; }
    }
}
