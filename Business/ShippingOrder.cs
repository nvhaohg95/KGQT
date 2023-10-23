﻿using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.EntityFrameworkCore;

namespace KGQT.Business
{
    public static class ShippingOrder
    {
        #region Get List
        public static object[] GetList(int status, string ID, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20, string? userName = "")
        {

            using (var db = new nhanshiphangContext())
            {
                int pageNum = page - 1;
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

                query = query.OrderByDescending(x => x.CreatedDate)
                    .Skip(pageNum * pageSize)
                    .Take(pageSize);
                var lstData = query.ToList();
                decimal count = query.Count();
                decimal totalPage = Math.Ceiling(count / pageSize);
                return new object[] { lstData , totalPage};
            }
        }

        #endregion

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
                oder.InsurancePrice = totalPrice * 0.05;
                db.Update(oder);
                return db.SaveChanges() > 0;
            }
        }

       
        #endregion
    }
}
