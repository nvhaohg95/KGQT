using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_FeePackage
    {
        public int ID { get; set; }
        public double? VolumeFrom { get; set; }
        public double? VolumeTo { get; set; }
        public double? Price { get; set; }
        public bool? IsHide { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
