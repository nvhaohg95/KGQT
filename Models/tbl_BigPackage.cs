using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_BigPackage
    {
        public int ID { get; set; }
        public string? BigPackageCode { get; set; }
        public DateTime? BigPackageDateExport { get; set; }
        public int? BigPackageShippingMethod { get; set; }
        public string? BigPackageShippingMethodName { get; set; }
        public DateTime? BigPackageDateExpectation { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
