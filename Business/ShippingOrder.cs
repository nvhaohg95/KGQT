using KGQT.Areas.Admin.Models.temp;
using KGQT.Business.Base;
using KGQT.Models;
using Microsoft.EntityFrameworkCore;

namespace KGQT.Business
{
    public static class ShippingOrder
    {
        public static List<tmpShippingOrder> GetList(string search, string uid, int page, int pageSize)
        {
            
            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tmpShippingOrder>();
                var shippingOrders = new List<tbl_ShippingOrder>();
                shippingOrders = db.tbl_ShippingOrders.ToList();
                if (shippingOrders.Count > 0)
                {
                    foreach (var item in shippingOrders)
                    {
                        var data = new tmpShippingOrder()
                        {
                            ID = item.ID,
                            ShippingOrderCode = item.ShippingOrderCode,
                            CreatedDate = item.CreatedDate != null ? item.CreatedDate.Value.ToShortDateString() : "",
                            DateExpectation = item.DateExpectation != null ? item.DateExpectation.Value.ToShortDateString() : "",
                            CreatedBy = item.CreatedBy,
                            Username = item.Username,
                            TotalPrice = item.TotalPrice,
                            Status = item.Status,
                            StatusName = item.Status == 1 ? "Chưa giao":"Đã giao"
                        };
                        lstData.Add(data);
                    }
                }
                return lstData;
            }
        }

        public static List<tmpShippingOrder> GetList(int? status, DateTime? fromDate, DateTime? toDate, string? searchText,int? page, int? pageSize)
        {

            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tmpShippingOrder>();
                var shippingOrders = new List<tbl_ShippingOrder>();
                var query = db.tbl_ShippingOrders.AsQueryable();
                if (status != 0)
                    query = query.Where(x => x.Status == status);
                if (fromDate != null && toDate != null)
                    query = query.Where(x => x.CreatedDate >= fromDate && x.CreatedDate <= toDate);
                if (!string.IsNullOrEmpty(searchText))
                    query = query.Where(x => x.Username.Contains(searchText));

                shippingOrders = query.ToList();
                if (shippingOrders.Count > 0)
                {
                    foreach (var item in shippingOrders)
                    {
                        var data = new tmpShippingOrder()
                        {
                            ID = item.ID,
                            ShippingOrderCode = item.ShippingOrderCode,
                            CreatedDate = item.CreatedDate != null ? item.CreatedDate.Value.ToShortDateString() : "",
                            DateExpectation = item.DateExpectation != null ? item.DateExpectation.Value.ToShortDateString() : "",
                            CreatedBy = item.CreatedBy,
                            Username = item.Username,
                            TotalPrice = item.TotalPrice,
                            Status = item.Status,
                            StatusName = item.Status == 1 ? "Chưa giao" : "Đã giao"
                        };
                        lstData.Add(data);
                    }
                }
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
