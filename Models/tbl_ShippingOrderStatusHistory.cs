using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_ShippingOrderStatusHistory
    {
        public int ID { get; set; }
        public int? ShippingOrderID { get; set; }
        public string? ShippingOrderCode { get; set; }
        public int? ShippingOrderStatus { get; set; }
        public string? ShippingOrderStatusName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
