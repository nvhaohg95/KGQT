using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;

namespace KGQT.Business
{
    public class PointsBusiness
    {
        public static object[] GetPage(string userName, string orderID, int type, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            using (var db = new nhanshiphangContext())
            {
                int total = 0;
                int totalPage = 0;
                IQueryable<tbl_Point> query = db.tbl_Points;
                if (!string.IsNullOrEmpty(userName))
                    query = query.Where(x => x.Username == userName);
                if (!string.IsNullOrEmpty(orderID))
                    query = query.Where(x => x.OrderID == orderID);
                if (type > -1)
                    query = query.Where(x => x.Type == type);
                if (fromDate != null)
                    query = query.Where(x => x.CreatedDate >= fromDate);
                if (toDate != null)
                {
                    toDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(x => x.CreatedDate < toDate);
                }
                total = query.Count();
                if (total > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                    query = query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize);
                }
                var oData = query.GroupJoin(db.tbl_Packages, a => a.OrderID, b => b.PackageCode, (a,b) => new { point = a, pack = b })
                    .SelectMany(x => x.pack.DefaultIfEmpty(), (x, y) => new tempPoints
                    {
                        CreatedDate = x.point.CreatedDate,
                        HContent = x.point.HContent,
                        Point = x.point.Point,
                        Type = x.point.Type,
                        PointLeft = x.point.PointLeft,
                        Note = y != null ? y.Note : "",
                    }).ToList();
                return new object[] { oData, total, totalPage };
            }
        }

        //Type = 0: + điểm;Type = 1: - điểm
        public static bool Insert(int uid, string username, string orderId, string content, int point, int type, int poinLeft, string createdBy)
        {
            tbl_Point pay = new tbl_Point();
            pay.UID = uid;
            pay.Username = username;
            pay.OrderID = orderId;
            pay.HContent = content;
            pay.Type = type;
            pay.Point = point;
            pay.PointLeft = poinLeft;
            pay.CreatedDate = DateTime.Now;
            pay.CreatedBy = createdBy;
            pay.Status = 1;
            pay.IsActive = 1;
            return BusinessBase.Add(pay);
        }

    }
}
