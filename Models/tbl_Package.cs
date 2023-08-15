using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Package
    {
        public int ID { get; set; }
        public string? PackageCode { get; set; }
        public string? PackageOrderHangCode { get; set; }
        public int? OrderShopID { get; set; }
        public string? OrderShopCode { get; set; }
        public string? ShippingCode { get; set; }
        public string? Barcode { get; set; }
        public string? BarCodeIMG { get; set; }
        /// <summary>
        /// 1. Chưa về
        /// 2. Đã về
        /// 3. Đã giao
        /// </summary>
        public int? Status { get; set; }
        public double? Volume { get; set; }
        public double? VolumeLength { get; set; }
        public double? VolumeHeight { get; set; }
        public double? VolumeWidth { get; set; }
        public double? VolumePrice { get; set; }
        public double? VolumeExchange { get; set; }
        public double? Package { get; set; }
        public double? PackageLength { get; set; }
        public double? PackageHeight { get; set; }
        public double? PackageWidth { get; set; }
        public double? PackagePrice { get; set; }
        public double? PackageExchange { get; set; }
        public double? PackageWeight { get; set; }
        public double? PackageWeightCompare { get; set; }
        public double? Length { get; set; }
        public double? Height { get; set; }
        public double? Width { get; set; }
        public double? WeightExchange { get; set; }
        public double? WeightPrice { get; set; }
        public DateTime? SGWarehouseDate { get; set; }
        public DateTime? NgayGiaoHang { get; set; }
        public DateTime? YCGWarehouseDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public int? UID { get; set; }
        public string? Username { get; set; }
        public DateTime? DateExport { get; set; }
        public double? PriceInWareHouse { get; set; }
        public bool? IsChangePrice { get; set; }
        public double? Price { get; set; }
        public int? Amount { get; set; }
        public double? TotalPrice { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
