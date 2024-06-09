using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Point
    {
        public int ID { get; set; }
        public int? UID { get; set; }
        public string Username { get; set; } = null!;
        public string OrderID { get; set; } = null!;
        public int Point { get; set; }
        public string? HContent { get; set; }
        public int PointLeft { get; set; }
        /// <summary>
        /// 1: trừ
        /// 2: cộng
        /// 
        /// </summary>
        public int Type { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public int? IsActive { get; set; }
    }
}
