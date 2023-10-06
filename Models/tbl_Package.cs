using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Package
    {
        public int ID { get; set; }
        public string? PackageCode { get; set; }
        public int? ShippingOrderID { get; set; }
        public string? Barcode { get; set; }
        /// <summary>
        /// 1. Chưa về
        /// 2. Đã về
        /// 3. Đã giao
        /// </summary>
        public int? Status { get; set; }
        public double? AirPackageWeight { get; set; }
        public double? AirPackageLength { get; set; }
        public double? AirPackageHeight { get; set; }
        public double? AirPackageWidth { get; set; }
        public double? AirPackagePrice { get; set; }
        public double? AirPackageWeightExchange { get; set; }
        public double? WoodPackageWeight { get; set; }
        public double? WoodPackageLength { get; set; }
        public double? WoodPackageHeight { get; set; }
        public double? WoodPackageWidth { get; set; }
        public double? WoodPackagePrice { get; set; }
        public double? WoodPackageWeightExchange { get; set; }
        public double? PackageWeight { get; set; }
        public double? PackageWeightCompare { get; set; }
        public double? Length { get; set; }
        public double? Height { get; set; }
        public double? Width { get; set; }
        public double? WeightExchange { get; set; }
        public double? Weight { get; set; }
        public double? WeightPrice { get; set; }
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
        public double? PriceInWareHouse { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public int? SearchBaiduTimes { get; set; }
        public int? SearchBaiduTimesEdit { get; set; }
        public DateTime? DateExpectation { get; set; }
        public DateTime? DateExpectationEdit { get; set; }
        public int? BigPackageID { get; set; }
    }
}
