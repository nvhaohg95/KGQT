using KGQT.Models;

namespace KGQT.Business
{
    public class ShippingOrder
    {
        public List<tbl_ShippingOrder> GetList(string search, string uid, int page, int pageSize)
        {
            using (var db = new nhanshiphangContext())
            {
                return db.tbl_ShippingOrders.ToList();
            }
        }
    }
}
