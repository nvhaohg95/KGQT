namespace KGQT.Models.temp
{
    public class CassoModel
    {
        public int error { get; set; }
        public List<CassoData> data { get; set; }
    }
    public class CassoData
    {
        public string when { get; set; }
        public int amount { get; set; }
        public string description { get; set; }
        public int cusum_balance { get; set; }
        public string tid { get; set; }
        public string subAccId { get; set; }
        public string orderOfTheDay { get; set; }
        public int id { get; set; }
    }

}
