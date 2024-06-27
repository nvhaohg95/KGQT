using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Package
    {
        public int ID { get; set; }
        public Guid RecID { get; set; }
        public string PackageCode { get; set; } = null!;
        public int? BigPackage { get; set; }
        public string? TransID { get; set; }
        public string? Barcode { get; set; }
        /// <summary>
        /// 1. Chưa về
        /// 2. Đã về
        /// 3. Đã giao
        /// </summary>
        public int Status { get; set; }
        public int MovingMethod { get; set; }
        public string? Weight { get; set; }
        public string? Length { get; set; }
        public string? Width { get; set; }
        public string? Height { get; set; }
        public string? WeightExchange { get; set; }
        public string? WeightReal { get; set; }
        public string? WeightPrice { get; set; }
        public bool? IsAirPackage { get; set; }
        public string? AirPackagePrice { get; set; }
        public bool? IsWoodPackage { get; set; }
        public string? WoodPackagePrice { get; set; }
        public bool? IsInsurance { get; set; }
        public string? IsInsurancePrice { get; set; }
        public bool? IsBrand { get; set; }
        public string? SurCharge { get; set; }
        public string? MoreCharge { get; set; }
        public string? TotalPrice { get; set; }
        public string? Declaration { get; set; }
        public string? DeclarePrice { get; set; }
        public string? WareHouse { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ComfirmDate { get; set; }
        /// <summary>
        /// Chờ giao
        /// </summary>
        public DateTime? ExportedCNWH { get; set; }
        /// <summary>
        /// Đang giao
        /// </summary>
        public DateTime? TransportingToSGWH { get; set; }
        /// <summary>
        /// Ngày bắt đầu vận chuyển
        /// </summary>
        public DateTime? ImportedSGWH { get; set; }
        /// <summary>
        /// Ngày nhận hàng
        /// </summary>
        public DateTime? ReceivedDate { get; set; }
        public int? UID { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Note { get; set; }
        public string? CanceledBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public int? SearchBaiduTimes { get; set; }
        public string? DateExpectation { get; set; }
        public bool? Exported { get; set; }
        public bool? AutoQuery { get; set; }
        public int? TrackingShipping { get; set; }
        public bool? IsImport { get; set; }
    }
}
