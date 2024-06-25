using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Schedules
    {
        public Guid RecID { get; set; }
        public int? TaskID { get; set; }
        public string? TaskName { get; set; }
        public int? Hours { get; set; }
        public int? Minute { get; set; }
        public int? Seccon { get; set; }
        public bool? IsActive { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
