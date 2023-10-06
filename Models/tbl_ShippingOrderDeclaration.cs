﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KGQT.Models
{
    public partial class tbl_ShippingOrderDeclaration
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int? ShippingOrderID { get; set; }
        public string? ShippingOrderCode { get; set; }
        public string? ProducName { get; set; }
        public string? ProductLink { get; set; }
        public double? ProductPrice { get; set; }
        public double? PriceVND { get; set; }
        public int? ProductQuantity { get; set; }
        public int? ProductType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
