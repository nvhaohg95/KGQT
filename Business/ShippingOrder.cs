using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.EntityFrameworkCore;
using System.Composition;

namespace KGQT.Business
{
    public static class ShippingOrder
    {
        #region Get count
        public static tbl_ShippingOrder GetOne(int id)
        {
            using (var db = new nhanshiphangContext())
            {
                return db.tbl_ShippingOrders.FirstOrDefault(x => x.ID == id);
            }
        }

        public static tbl_ShippingOrder CheckOrderInStock(string username, int method, DateTime fromDate, DateTime toDate, DateTime exportStart, DateTime exportEnd)
        {
            using (var db = new nhanshiphangContext())
            {
                try
                {
                    var query = db.tbl_ShippingOrders.Where(x =>x.Status == 1 && x.ShippingMethod == method && x.Username.ToLower() == username.ToLower()
                    && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.ChinaExportDate >= exportStart && x.ChinaExportDate <= exportEnd);

                    var a = query.FirstOrDefault();
                    return a;
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
        }
        public static object[] GetAllStatus(string username)
        {
            using (var db = new nhanshiphangContext())
            {
                IQueryable<tbl_ShippingOrder> qry = db.tbl_ShippingOrders;
                if (!string.IsNullOrEmpty(username))
                    qry = qry.Where(x => x.Username == username);

                int st1 = qry.Where(x => x.Status == 1).Count();
                int st2 = qry.Where(x => x.Status == 2).Count();
                int total = qry.Count();
                return new object[] { st1, st2, total };
            }
        }
        #endregion

        #region Get List
        public static object[] GetPage(int status, string ID, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20, string? userName = "")
        {

            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tbl_ShippingOrder>();
                int totalPage = 1;
                int total = 0;
                IQueryable<tbl_ShippingOrder> query = db.tbl_ShippingOrders;
                if (!string.IsNullOrEmpty(userName))
                    query = query.Where(x => x.Username == userName);

                if (status > 0)
                    query = query.Where(x => x.Status == status);

                if (!string.IsNullOrEmpty(ID))
                    query = query.Where(x => x.ShippingOrderCode == ID);

                if (fromDate != null)
                    query = query.Where(x => x.CreatedDate >= fromDate);

                if (toDate != null)
                    query = query.Where(x => x.CreatedDate <= toDate);

                total = query.Count();
                if (total > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));

                    lstData = query.OrderByDescending(x => x.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize).ToList();
                }

                return new object[] { lstData, total, totalPage };
            }
        }

        #endregion

        #region CRUD
        public static bool Add(tbl_ShippingOrder model)
        {
            return BusinessBase.Add(model);
        }


        #endregion
    }
}
