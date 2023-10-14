using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.EntityFrameworkCore;

namespace KGQT.Business
{
    public static class ShippingOrder
    {
        public static List<tbl_ShippingOrder> GetList(int? status, DateTime? fromDate, DateTime? toDate, string? searchText, string userName = "", int page = 0, int pageSize = 20)
        {

            using (var db = new nhanshiphangContext())
            {
                IQueryable<tbl_ShippingOrder> query = db.tbl_ShippingOrders;
                if (status > 0)
                    query = query.Where(x => x.Status == status);

                if (fromDate != null)
                    query = query.Where(x => x.CreatedDate >= fromDate);

                if (toDate != null)
                    query = query.Where(x => x.CreatedDate <= toDate);

                if (!string.IsNullOrEmpty(searchText))
                    query = query.Where(x => x.ShippingOrderCode.Contains(searchText));

                query = query.OrderByDescending(x => x.CreatedDate).Skip(page * pageSize).Take(pageSize);
                var lstData = query.ToList();
                return lstData;
            }
        }

        #region CRUD
        public static bool UpdateFeeIsurance(int id)
        {
            using (var db = new nhanshiphangContext())
            {
                var oder = db.tbl_ShippingOrders.FirstOrDefault(x => x.ID == id);

                if (oder == null) return false;

                var conf = db.tbl_Configurations.FirstOrDefault();

                if (conf == null) return false;

                double totalPrice = (double)db.tbl_ShippingOrderDeclarations.Where(x => x.ShippingOrderID == id).Sum(x => x.PriceVND);
                oder.IsInsurancePrice = totalPrice * 0.05;
                db.Update(oder);
                return db.SaveChanges() > 0;
            }
        }

       
        #endregion
    }
}
