using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_ShippingOrder
    {
        public int ID { get; set; }
        public string ShippingOrderCode { get; set; } = null!;
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? ShippingMethod { get; set; }
        public string? ShippingMethodName { get; set; }
        public double? Weight { get; set; }
        public double? WeightPrice { get; set; }
        public bool? IsWoodPackage { get; set; }
        public double? WoodPackagePrice { get; set; }
        public bool? IsAirPackage { get; set; }
        public double? AirPackagePrice { get; set; }
        public bool? IsInsurance { get; set; }
        public double? InsurancePrice { get; set; }
        public int Status { get; set; }
        public int? PaymentMethod { get; set; }
        public double? TotalPrice { get; set; }
        public DateTime? DateExpectation { get; set; }
        public DateTime? DateExpectationEdit { get; set; }
        public double? Currency { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
