using DocumentFormat.OpenXml.Wordprocessing;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using OfficeOpenXml.Table.PivotTable;
using System.Security.Cryptography.Pkcs;
using Serilog;
using ILogger = Serilog.ILogger;
using DocumentFormat.OpenXml.Office2010.Excel;
using KGQT.Base;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Spreadsheet;

namespace KGQT.Business
{
    public static class NotificationBusiness
    {
        private static readonly ILogger _log = Log.ForContext(typeof(NotificationBusiness));

        public static object[] GetPage(int receivedID, int status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20, bool isAdmin = false)
        {
            using (var db = new nhanshiphangContext())
            {
                var datas = new List<tbl_Notification>();
                int count = 0;
                int totalPage = 0;
                var query = db.tbl_Notifications.AsQueryable();
                if (isAdmin)
                    query = db.tbl_Notifications.Where(x => x.IsForAdmin == true);
                else
                    query = query.Where(x => x.ReceivedID == receivedID);

                if (status > -1)
                    query = query.Where(x => x.Status == status);

                if (fromDate != null)
                    query = query.Where(x => x.CreatedDate >= fromDate);
                if (toDate != null)
                {
                    toDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(x => x.CreatedDate < toDate);
                }

                count = query.Count();
                if (count > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)count / pageSize));
                    datas = query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }
                return new object[] { datas, count, totalPage };
            }
        }

        public async static Task<bool> Insert(int senderID, string senderName, int? reciverID, string reciverName, int orderID, string? orderCode, string message, string messageMobile, int notiType, string url, string createdBy, bool isForAdmin = false)
        {
            try
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
                        if (string.IsNullOrEmpty(data.Url))
                        {
                            data.Url = GetUrlDefault(data.ID, isForAdmin);
                            db.Update(data);
                            db.SaveChanges();
                        }
                        var user = BusinessBase.GetOne<tbl_Account>(x => x.ID == reciverID);
                        if (user != null && !string.IsNullOrEmpty(user.TokenDevice))
                        {
                            _ = Task.Run(async () =>
                            {
                                Dictionary<string, string> datas = new Dictionary<string, string>();
                                datas.Add("data", JsonConvert.SerializeObject(data));
                                await Helper.SendFCMAsync(messageMobile, user.TokenDevice, datas, user.ID);
                            });
                        }
                        var follower = db.tbl_ZaloFollewers.FirstOrDefault(x => x.Username == user.Username);
                        if (follower != null)
                        {
                            _ = Task.Run(async () =>
                            {
                                _ = ZaloCommon.SendMessage(follower?.user_id, messageMobile);
                            });
                        }
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi tạo thông báo", ex.Message);
                return false;
            }
        }

        public static int GetTotal(int id, bool isAdmin = false)
        {
            int total = 0;
            using (var db = new nhanshiphangContext())
            {
                if (isAdmin)
                    total = db.tbl_Notifications.Count(x => x.IsForAdmin == true && x.Status == 0);
                else
                    total = db.tbl_Notifications.Count(x => x.ReceivedID == id && x.Status == 0);
                return total;
            }
        }

        public static object[] GetDetail(int id, string userLogin)
        {
            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tbl_Notification>();
                var data = db.tbl_Notifications.FirstOrDefault(x => x.ID == id);
                if (data != null)
                {
                    data.Status = 1;
                    data.ModifiedDate = DateTime.Now;
                    data.ModifiedBy = userLogin;
                    db.Update(data);
                    db.SaveChanges();
                    lstData.Add(data);
                }
                return new object[] { lstData, lstData.Count, lstData.Count };
            }
        }

        public static bool UpdateStatus(int ID, string userLogin)
        {
            if (!string.IsNullOrEmpty(userLogin) || ID < 0)
            {
                using (var db = new nhanshiphangContext())
                {
                    var data = db.tbl_Notifications.FirstOrDefault(x => x.ID == ID);
                    if (data != null)
                    {
                        data.Status = 1;
                        data.ModifiedBy = userLogin;
                        data.ModifiedDate = DateTime.Now;
                        db.Update(data);
                        int kq = db.SaveChanges();
                        return kq > 0;
                    }
                }
            }
            return false;
        }

        public static string GetUrlDefault(int ID, bool isAdmin = false)
        {
            if (isAdmin)
                return "/Admin/Notification/Detail?ID=" + ID;
            return "/Notification/Detail?ID=" + ID;
        }

        public static DataReturnModel<tbl_Notification> GetByID(int id, string userLogin)
        {
            var result = new DataReturnModel<tbl_Notification>();
            using (var db = new nhanshiphangContext())
            {
                var data = db.tbl_Notifications.FirstOrDefault(x => x.ID == id);
                if (data != null)
                {
                    data.Status = 1;
                    data.ModifiedBy = userLogin;
                    data.ModifiedDate = DateTime.Now;
                    db.Update(data);
                    db.SaveChanges();
                }
                result.IsError = false;
                result.Data = data;
                return result;
            }
        }
        public static DataReturnModel<bool> SendNotiForUser(string strUserNames, string contents, bool sendToAll, string userLogin)
        {
            DataReturnModel<bool> result = new();
            try
            {
                if (string.IsNullOrEmpty(strUserNames) && !sendToAll)
                {
                    result.IsError = true;
                    result.Message = "Người nhận không được bỏ trống!";
                    result.Data = false;
                    return result;
                }
                if (string.IsNullOrEmpty(contents))
                {
                    result.IsError = true;
                    result.Message = "Nội dung không được bỏ trống!";
                    result.Data = false;
                    return result;
                }
                using (var db = new nhanshiphangContext())
                {
                    var admin = AccountBusiness.GetInfo(-1, userLogin);
                    List<tbl_Account> lstUser = new();
                    if (sendToAll)
                    {
                        lstUser = db.tbl_Accounts.Where(x => x.IsActive == true).ToList();

                        Task task = new Task(() =>
                        {
                            ZaloCommon.SendMessageToAllV2(contents);
                        });
                        task.Start();
                    }
                    else
                    {
                        List<string> userNames = strUserNames.Split(";").Select(x => x.ToLower()).Distinct().ToList();
                        lstUser = db.tbl_Accounts.Where(x => x.IsActive == true && userNames.Contains(x.Username)).ToList();
                        var zalo = db.tbl_ZaloFollewers.Where(x => userNames.Contains(x.Username)).ToList();
                        if (zalo != null && zalo.Count > 0)
                        {
                            Task task = new Task(() =>
                            {
                                foreach (var item in zalo)
                                {
                                    ZaloCommon.SendMessage(item.user_id, contents);
                                }
                            });
                            task.Start();
                        }
                    }
                    if (lstUser.Count > 0)
                    {
                        foreach (var user in lstUser)
                        {
                            try
                            {
                                var data = GenNoti(admin.ID, admin.FullName, user.ID, user.FullName, contents, contents, 10, userLogin);
                                db.Add(data);
                                if (user != null && !string.IsNullOrEmpty(user.TokenDevice))
                                {
                                    _ = Task.Run(async () =>
                                    {
                                        Dictionary<string, string> datas = new Dictionary<string, string>();
                                        datas.Add("data", JsonConvert.SerializeObject(data));
                                        await Helper.SendFCMAsync(contents, user.TokenDevice, datas, user.ID);
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                _log.Error("Lỗi gửi thông báo", ex.Message);
                                continue;
                            }
                        }
                        db.SaveChanges();

                        result.IsError = false;
                        result.Message = "Gửi thành công!";
                        result.Data = true;
                        return result;
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Không tìm thấy người nhận!";
                        result.Data = false;
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi gửi thông báo", ex.Message);
                result.IsError = true;
                result.Data = false;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                return result;
            }
        }


        public static tbl_Notification GenNoti(int senderID, string senderName, int? reciverID, string reciverName, string message, string messageMobile, int notiType, string createdBy)
        {
            var data = new tbl_Notification()
            {
                SenderID = senderID,
                SenderUsername = senderName,
                ReceivedID = reciverID,
                ReceivedUsername = reciverName,
                OrderID = -1,
                OrderCode = "",
                Message = message,
                Message2 = messageMobile,
                IsForAdmin = false,
                NotifType = notiType,
                Status = 0,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };

            if (string.IsNullOrEmpty(data.Url))
                data.Url = GetUrlDefault(data.ID);
            return data;
        }

    }
}
