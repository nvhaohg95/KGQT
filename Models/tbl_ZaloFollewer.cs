using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_ZaloFollewer
    {
        public Guid RecID { get; set; }
        public string? user_id { get; set; }
        public string? display_name { get; set; }
        public string? phone { get; set; }
        public string? address { get; set; }
        public string? Username { get; set; }
        public bool? SendRequest { get; set; }
        public DateTime? SendRequestDate { get; set; }
        public int? SendRequestTimes { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
