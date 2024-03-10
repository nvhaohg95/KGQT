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
        public int? PaymentMethod { get; set; }
        public int Status { get; set; }
        public string Weight { get; set; }
        public string WeightPrice { get; set; }
        public bool? IsWoodPackage { get; set; }
        public string WoodPackagePrice { get; set; }
        public bool? IsAirPackage { get; set; }
        public string AirPackagePrice { get; set; }
        public bool? IsInsurance { get; set; }
        public string InsurancePrice { get; set; }
        public string SurCharge { get; set; }
        public string TotalPrice { get; set; }
        public string Currency { get; set; }
        public DateTime? DateExpectation { get; set; }
        public DateTime? DateExpectationEdit { get; set; }
        public DateTime? ChinaExportDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
