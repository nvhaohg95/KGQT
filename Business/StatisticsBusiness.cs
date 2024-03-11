using DocumentFormat.OpenXml.Vml;
using KGQT.Commons;
using KGQT.Models;
using MimeKit.Encodings;

namespace KGQT.Business
{
    public class StatisticsBusiness
    {
        public static object[] UserWallet(int page, int pageSize)
        {
            using (var db = new nhanshiphangContext())
            {
                IQueryable<tbl_Account> query = db.tbl_Accounts
                    .Where(x => x.Wallet != 0);

                int total = query.Count();
                var lstData = query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                int totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                return new object[] { lstData, total, totalPage };

            }
        }

        public static object[] Revenue(DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            using (var db = new nhanshiphangContext())
            {
                IQueryable<tbl_ShippingOrder> query = db.tbl_ShippingOrders
                    .Where(x => x.Status == 2 && x.CreatedDate >= startDate && x.CreatedDate <= endDate);

                int total = query.Count();
                var lstData = query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                int totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                return new object[] { lstData, total, totalPage };

            }
        }

        public static object[] PackageWeight(DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            using (var db = new nhanshiphangContext())
            {
                IQueryable<tbl_Package> query = db.tbl_Packages
                    .Where(x => x.Weight > 0 && x.CreatedDate >= startDate && x.CreatedDate <= endDate);

                int total = query.Count();
                var lstData = query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                int totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                return new object[] { lstData, total, totalPage };

            }
        }

        public static Dictionary<string, double> CustomerDebt(DateTime? startDate, DateTime? endDate)
        {
            using (var db = new nhanshiphangContext())
            {
                IQueryable<tbl_ShippingOrder> query = db.tbl_ShippingOrders
                    .Where(x => x.Status == 1 && x.CreatedDate >= startDate && x.CreatedDate <= endDate);

                var a = query.AsEnumerable().GroupBy(x => x.Username).ToDictionary(x => x.Key, y => y.AsEnumerable().Sum(x => Converted.ToDouble(x.TotalPrice)));
                return a;
            }
        }
    }
}
