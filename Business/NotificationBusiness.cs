using KGQT.Models;
using OfficeOpenXml.Table.PivotTable;
using System.Security.Cryptography.Pkcs;

namespace KGQT.Business
{
    public static class NotificationBusiness
    {

        public static object[] GetPage(int id,int status, DateTime? fromDate, DateTime? toDate, int page = 1 , int pageSize = 20,bool isAdmin = false)
        {
            using (var db = new nhanshiphangContext())
            {
                var datas = new List<tbl_Notification>();
                int count = 0;
                int totalPage = 0;
                var query = db.tbl_Notifications.AsQueryable();
                if(isAdmin)
                    query = db.tbl_Notifications.Where(x => x.IsForAdmin == true);
                else 
                    query = query.Where(x => x.ReceivedID == id);

                if(status > 0)
                    query = query.Where(x => x.Status == status);
                if (fromDate != null)
                    query = query.Where(x => x.CreatedDate >= fromDate);
                if (toDate != null)
                    query = query.Where(x => x.CreatedDate <= toDate);

                count = query.Count();
                if (count > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)count / pageSize));
                    datas = query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }
                return new object[] { datas, count, totalPage };
            }
        }
        public static bool Insert(int senderID, string senderName, int? reciverID, string reciverName, int orderID, string? orderCode, string message, int notiType, string createdBy, bool isForAdmin = false)
        {
            using (var db = new nhanshiphangContext())
            {
                var data = new tbl_Notification()
                {
                    SenderID = senderID,
                    SenderUsername = senderName,
                    ReceivedID = reciverID,
                    ReceivedUsername = reciverName,
                    OrderID = orderID,
                    OrderCode = orderCode,
                    Message = message,
                    IsForAdmin = isForAdmin,
                    NotifType = notiType,
                    Status = 0,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                };
                db.Add(data);
                int kq = db.SaveChanges();
                if (kq > 0)
                    return true;
            }
            return false ;
        }
        
        public static int GetTotal(int id, bool isAdmin = false)
        {
            int count = 0;
            using (var db = new nhanshiphangContext())
            {
                if (isAdmin)
                    count = db.tbl_Notifications.Count(x => x.IsForAdmin == true && x.Status == 0);
                else
                    count = db.tbl_Notifications.Count(x => x.ReceivedID == id && x.Status == 0);
            }
            return count;
        }
    }
}
