using DocumentFormat.OpenXml.Vml;
using KGQT.Models;

namespace KGQT.Business
{
    public class StatisticsBusiness
    {
        public static object[] Revenue(DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            using (var db = new nhanshiphangContext())
            {
                IQueryable<tbl_ShippingOrder> query = db.tbl_ShippingOrders
                    .Where(x => x.Status == 2 && x.CreatedDate >= startDate && x.CreatedDate <= endDate);
                    
                int total = query.Count();
                var lstData = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                int totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                return new object[] { lstData, total, totalPage };

            }
        }
    }
}
