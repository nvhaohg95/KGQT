using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_SystemLog
    {
        public Guid ID { get; set; }
        public int? Type { get; set; }
        public string? Content { get; set; }
        public string? OldValue { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
