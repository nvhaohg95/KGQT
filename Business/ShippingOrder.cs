using KGQT.Business.Base;
using KGQT.Models;

namespace KGQT.Business
{
    public static class ShippingOrder
    {
        #region Select
        public static List<tbl_ShippingOrder> GetList(string search, string uid, int page, int pageSize)
        {
            using (var db = new nhanshiphangContext())
            {
                return db.tbl_ShippingOrders.ToList();
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
                oder.IsInsurancePrice = totalPrice * 0.05;
                db.Update(oder);
                return db.SaveChanges() > 0;
            }
        }
        #endregion
    }
}
