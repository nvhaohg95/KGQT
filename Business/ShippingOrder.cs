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
      
        #endregion
    }
}
