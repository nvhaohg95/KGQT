using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_CheckSmallPackage
    {
        public int ID { get; set; }
        public string? OrderShopCode { get; set; }
        public string? Barcode { get; set; }
        public string? OrderCodeCompare { get; set; }
        public string? Barcodecompare { get; set; }
        public string? Username { get; set; }
        public bool? IsHide { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
