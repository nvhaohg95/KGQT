namespace KGQT.Models.temp
{
    public class OrderShopModel
    {
        public int ID { get; set; }
        public int Status { get; set; }
        public string? SubStatus { get; set; }
        public string? OrderShopCode { get; set; }
        public bool IsShip { get; set; }
        public string? anhsanpham { get; set; }
        public string? OrderCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? TotalPriceVND { get; set; }
        public string? Deposit { get; set; }
        public bool IsConfirm { get; set; }
        public bool IsMoving { get; set; }
        public string? AcceptDate { get; set; }
        public string? CompleteDate { get; set; }
        public string? BeforeBuyDate { get; set; }
        public string? BuyDate { get; set; }
        public string? TQwarehouseDate { get; set; }
        public string? VNwarehouseDate { get; set; }
        public string? YcRequestDate { get; set; }
        public string? DateExport { get; set; }
        public bool IsWaiting { get; set; }
        public bool IsCheckProduct { get; set; }
    }

    public class OrderShopModelProtable
    {
        public int ID { get; set; }
        public string OrderShopCode { get; set; }
        public string CustomeName { get; set; }
        public int Status { get; set; }
        public bool IsMoving { get; set; }
        public string TotalPriceVND { get; set; }
        public string anhsanpham { get; set; }
        public string AcceptDate { get; set; }
        public string CreatedOn { get; set; }
    }

    public class ListPackageSG
    {
        public int ID { get; set; }
        public string OrderShopCode { get; set; }
        public string TotalPriceVND { get; set; }
        public string Deposit { get; set; }
        public int Total { get; set; }
    }

}
