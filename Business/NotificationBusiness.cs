using KGQT.Models;
using OfficeOpenXml.Table.PivotTable;
using System.Security.Cryptography.Pkcs;

namespace KGQT.Business
{
    public static class NotificationBusiness
    {

        public static object[] GetPage(int userID, int page , int pageSize = 20)
        {
            using (var db = new nhanshiphangContext())
            {
                var query = db.tbl_Notifications.Where(x => x.ReceivedID == userID);
                var count = query.Count();
                var totalPage = 0;
                var datas = new List<tbl_Notification>();
                if(count > 0)
                {
                    totalPage = count / pageSize;
                    datas = query.Skip((page - 1) * pageSize).Take(pageSize).OrderByDescending(x => x.CreatedDate).ToList();
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
    }
}
