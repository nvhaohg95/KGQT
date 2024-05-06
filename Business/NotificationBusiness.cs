using DocumentFormat.OpenXml.Wordprocessing;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using OfficeOpenXml.Table.PivotTable;
using System.Security.Cryptography.Pkcs;

namespace KGQT.Business
{
    public static class NotificationBusiness
    {
        public static object[] GetPage(int receivedID, int status, DateTime? fromDate, DateTime? toDate, int page = 1 , int pageSize = 20,bool isAdmin = false)
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
                    query = query.Where(x => x.ReceivedID == receivedID);

                if (status > -1)
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
        
        public async static Task<bool> Insert(int senderID, string senderName, int? reciverID, string reciverName, int orderID, string? orderCode, string message, string messageMobile, int notiType,string url, string createdBy, bool isForAdmin = false)
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
                    Message2 = messageMobile,
                    IsForAdmin = isForAdmin,
                    NotifType = notiType,
                    Status = 0,
                    Url = url,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                };
                
                db.Add(data);
                int kq = db.SaveChanges();
                if (kq > 0)
                {
                    if (string.IsNullOrWhiteSpace(data.Url))
                    {
                        data.Url = GetUrlDefault(data.ID, isForAdmin);
                        db.Update(data);
                        db.SaveChanges();
                    }
                    var user = BusinessBase.GetOne<tbl_Account>(x => x.ID == reciverID);
                    if (user != null && !string.IsNullOrEmpty(user.TokenDevice))
                    {
                        Helper.SendFCMAsync(message, user.TokenDevice, null);
                    }
                    return true;
                }
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

        public static object[] GetDetail(int id,string userName)
        {
            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tbl_Notification>();
                var data = db.tbl_Notifications.FirstOrDefault(x => x.ID == id);
                if (data != null)
                {
                    data.Status = 1;
                    data.ModifiedDate = DateTime.Now;
                    data.ModifiedBy = userName;
                    db.Update(data);
                    db.SaveChanges();
                    lstData.Add(data);
                }
                return new object[] { lstData, lstData.Count, lstData.Count };
            }
        }

        public static void UpdateStatus(int ID, string userName)
        {
            using(var db = new nhanshiphangContext())
            {
                var data = db.tbl_Notifications.FirstOrDefault(x => x.ID == ID);
                if(data != null)
                {
                    data.Status = 1;
                    data.ModifiedBy = userName;
                    data.ModifiedDate = DateTime.Now;
                    db.Update(data);
                    db.SaveChanges();
                }
            }
        }

        public static string GetUrlDefault(int ID,bool isAdmin = false)
        {
            if(isAdmin)
                return "/Admin/Notification/Detail?ID=" + ID;
            return "/Notification/Detail?ID=" + ID;
        }
    }
}
