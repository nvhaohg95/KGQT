using KGQT.Models;

namespace KGQT.Business
{
    public class StatisticsBusiness
    {
        public static List<tbl_ShippingOrder> Revenue(DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            using (var db = new nhanshiphangContext())
            {
                var data = db.tbl_ShippingOrders
                    .Where(x => x.Status == 2 && x.CreatedDate >= startDate && x.CreatedDate <= endDate)
                    .Skip(page * pageSize).Take(pageSize).ToList();
                return data;
            }
        }
    }
}
