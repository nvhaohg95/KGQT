using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_ComplainComment
    {
        public int ID { get; set; }
        public int? UID { get; set; }
        public int? ComplainID { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public bool? IsHide { get; set; }
    }
}
