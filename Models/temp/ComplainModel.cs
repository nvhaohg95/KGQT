namespace KGQT.Models.temp
{
    public class ComplainModel
    {
        public string OrderShopCode { get; set; }
        public int ComplainType { get; set; } = 0;
        public int ComplainFor { get; set; } = 1;
        public string Text { get; set; }
    }
}
