using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_ShippingMethodAddDate
    {
        public int ID { get; set; }
        public string? ShippingMethodName { get; set; }
        public int? ShippingMethod { get; set; }
        public int? DateAddition { get; set; }
        public double? LessKg { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
