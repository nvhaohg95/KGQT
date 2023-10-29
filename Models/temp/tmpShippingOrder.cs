namespace KGQT.Models.temp
{
    public class tmpShippingOrder
    {
        public int ID { get; set; }
        public string? ShippingOrderCode { get; set; }
        public string? DateExpectation { get; set; }
        public string? Username { get; set; }
        public double? TotalPrice { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public string? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

    }

    public class tempPackage
    {
        public int ID { get; set; }
        public string PackageCode { get; set; }
        public int Status { get; set; }
        public int MovingMethod { get; set; }
        public bool? IsWoodPackage { get; set; }
        public double? WoodPackagePrice { get; set; }
        public bool? IsAirPackage { get; set; }
        public double? AirPackagePrice { get; set; }
        public bool? IsInsurance { get; set; }
        public double? IsInsurancePrice { get; set; }
        public string? Declaration { get; set; }
        public double? DeclarePrice { get; set; }
        public string? WareHouse { get; set; }
        public string? Username { get; set; }
    }
}
