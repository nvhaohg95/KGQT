namespace KGQT.Models.temp
{
    public class tmpChinaOrderStatus
    {
        public bool success { get; set; }
        public string reason { get; set; }
        public List<Data> data { get; set; }
        public string exname { get; set; }
        public string ico { get; set; }
        public string phone { get; set; }
        public string url { get; set; }
        public string nu { get; set; }
        public string company { get; set; }
        public string ischeck { get; set; }
        public string status { get; set; }
        public string city { get; set; }
        public string cityfrom { get; set; }
    }

    public class Data
    {
        public string time { get; set; }
        public string context { get; set; }
    }
}
