using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_ZaloWebHook
    {
        public Guid RecID { get; set; }
        public string? app_id { get; set; }
        public string? event_name { get; set; }
        public string? timestamp { get; set; }
        public string? sender { get; set; }
        public string? recipient { get; set; }
        public string? msg_id { get; set; }
        public string? text { get; set; }
        public string? address { get; set; }
        public string? phone { get; set; }
        public string? name { get; set; }
        public int? status { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
