namespace KGQT.Models.temp
{
    public class BillFilter : PagingModel
    {
        public string? SearchText { get; set; }
        public int Type { get; set; }

    }

    public class BillModel
    {
        public int ID { get; set; }
        public int SoLuongKien { get; set; }
        public string DiaChiGiao { get; set; }
        public int LoaiYCG { get; set; }
        public string SDTTaixe { get; set; }
        public string CreatedOn { get; set; }
    }
}
