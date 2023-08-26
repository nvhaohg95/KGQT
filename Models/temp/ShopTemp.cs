namespace KGQT.Models.temp
{
    public class ShopTemp
    {
        public int ID { get; set; }
        public string ShopName { get; set; }
        public int SourceType { get; set; }
        public double TongTien_MoiShop { get; set; } = 0;
        public double TongTienHang_MoiShop_VND { get; set; } = 0;
        public double TongTienhang_MoiShop_NDT { get; set; } = 0;
        public double TongTienPhiMuaHang_MoiShop_VND { get; set; } = 0;
        public double TongTienPhiMuaHang_MoiShop_NDT { get; set; } = 0;
        public double TongTienPhiKiemDem_MoiShop_VND { get; set; } = 0;
        public double TongTienPhiBaoHiem_MoiShop_VND { get; set; } = 0;
        public double TongTienPhiBaoHiem_MoiShop_NDT { get; set; } = 0;
        public int TotalProduct { get; set; }
        public int TotalLink { get; set; }
        public bool IsCheckProduct { get; set; }
        public double Deposit { get; set; }
        public List<ProductTemp> Products { get; set; }
    }

    public class ProductTemp
    {
        public int ID { get; set; }
        public string LinkProduct { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public double U_PricecBuy { get; set; }
        public double U_PriceVN { get; set; }
        public double E_PriceBuy { get; set; }
        public double E_PriceVN { get; set; }
    }
}
