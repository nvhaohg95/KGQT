using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbll_ConfigurationNoti
    {
        public int ID { get; set; }
        public int? UID { get; set; }
        public string? Username { get; set; }
        public bool? CMTDonhang { get; set; }
        public bool? CMTKhieunai { get; set; }
        public bool? YCGH { get; set; }
        public bool? Khieunai { get; set; }
        public bool? Ruttien { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
