using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Comment
    {
        public int ID { get; set; }
        public int? UID { get; set; }
        public int? URole { get; set; }
        public string? Username { get; set; }
        public int? OrderShopID { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
