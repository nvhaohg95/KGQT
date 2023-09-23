using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_ShippingOrderDeclaration
    {
        public int ID { get; set; }
        public int? ShippingOrderID { get; set; }
        public string? ShippingOrderCode { get; set; }
        public string? ProducName { get; set; }
        public string? ProductLink { get; set; }
        public double? ProductPrice { get; set; }
        public int? ProductQuantity { get; set; }
        public int? ProductType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
