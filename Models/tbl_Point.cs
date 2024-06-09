using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Point
    {
        public int ID { get; set; }
        public int? UID { get; set; }
        public string Username { get; set; } = null!;
        public string Point { get; set; } = null!;
        public string? HContent { get; set; }
        public string PointLeft { get; set; } = null!;
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public int? IsActive { get; set; }
    }
}
