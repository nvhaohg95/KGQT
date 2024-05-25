using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_ZaloLog
    {
        public Guid RecID { get; set; }
        public int? action { get; set; }
        public int? error { get; set; }
        public string? message { get; set; }
        public string? user_id { get; set; }
        public string? user_name { get; set; }
        public string? template { get; set; }
        public string? context { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
