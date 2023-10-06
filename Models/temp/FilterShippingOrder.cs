using System.ComponentModel.DataAnnotations;

namespace KGQT.Models.temp
{

    public class FilterShippingOrder
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? TextSearch { get; set; }
        public List<EStatus> Status { get; set; }
        public List<ETimeSet> TimeSets { get; set; }

        public FilterShippingOrder()
        {
            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
            TextSearch = "";
            Status = new List<EStatus>()
            {
                new EStatus() { ID = 0, Name = "Tất cả"},
                new EStatus() { ID = 1, Name = "Chưa giao"},
                new EStatus() { ID = 2, Name = "Đang giao"}
            };
            TimeSets = new List<ETimeSet>()
            {
                new ETimeSet() { ID = "d", Name = "Hôm nay"},
                new ETimeSet() { ID = "w", Name = "Trong tuần"},
                new ETimeSet() { ID = "m", Name = "Trong tháng"},
                new ETimeSet() { ID = "y", Name = "Trong năm"}
            };
        }
    }


    public class EStatus
    {
        public int ID { get; set; }

        public string Name { get; set; }
    }
    public class ETimeSet
    {
        public string ID { get; set; }

        public string Name { get; set; }
    }
}
