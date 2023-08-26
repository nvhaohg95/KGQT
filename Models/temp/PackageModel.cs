namespace KGQT.Models.temp
{
    public class PackageModel
    {
        public int ID { get; set; }
        public string PackageOrderHangCode { get; set; }
        public string OrderShopCode { get; set; }
        public int OrderShopID { get; set; }
        public double PackageWeight { get; set; }
        public double? WeightExchange { get; set; }
        public DateTime DateExport { get; set; }
        public string ReceiveDate { get; set; }
        public int Status { get; set; }
    }

    public class PackageFilter : FilterModel
    {
        public string? OrderShopCode { get; set; }
        public int? Status { get; set; }
        public string? PackageOrderHangCode { get; set; }
    }
}
