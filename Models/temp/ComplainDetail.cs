namespace KGQT.Models.temp
{
    public class ComplainDetail
    {
        public int ID { get; set; }
        public string OrderShopCode { get; set; }
        public int? Status { get; set; }
        public string ComplainText { get; set; }
        public int? Type { get; set; }
        public string IMG { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
    }

    public class ComplainFilter : PagingModel
    {
        public int? OrderShopID { get; set; }
        public int? Type { get; set; }
        public int? Status { get; set; }
    }
}
