
using DocumentFormat.OpenXml.Wordprocessing;
using KGQT.Models;
using KGQT.Models.temp;
using Serilog;
using ILogger = Serilog.ILogger;

namespace KGQT.Business
{
    public static class ComplainBusiness
    {
        private static readonly ILogger _log = Log.ForContext(typeof(AccountBusiness));

        public static DataReturnModel<bool> Insert(tbl_Complain data)
        {
            DataReturnModel<bool> result = new();
            try
            {
                using (var db = new nhanshiphangContext())
                {
                    db.Add(data);
                    var kq = db.SaveChanges();
                    if (kq > 0)
                    {
                        result.IsError = false;
                        result.Data = true;
                        result.Message = "Yêu cầu của bạn đã được gửi!";
                        var user = AccountBusiness.GetInfo(-1, data.CreatedBy);
                        if (user != null)
                        {
                            string message = string.Format("Khách hàng <span class=\"fw-bold\">{0}</span> đã gửi 1 khiếu nại.", user.FullName);
                            string message2 = string.Format("Khách hàng {0} đã gửi 1 khiếu nại.", user.FullName);
                            string url = string.Format("/Admin/Complain/Index?ID={0}", data.ID);
                            NotificationBusiness.Insert(user.ID, user.FullName, -1, "Admin", -1, data.TransId, message, message2, 5, url, user.Username, true);
                        }
                    }
                    else
                    {
                        result.IsError = true;
                        result.Data = false;
                        result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi tạo complain", ex.Message);
                result.IsError = true;
                result.Data = false;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                return result;
            }

        }

        public static object[] GetList(int? ID,string transId, DateTime? fromDate, DateTime? toDate,int? status, int page, int pageSize = 10)
        {
            List<tbl_Complain> lstData = new();
            int count = 0;
            int totalPage = 0;

            using (var db = new nhanshiphangContext())
            {
                var query = db.tbl_Complains.AsEnumerable();
                if (ID > 0)
                    query = query.Where(x => x.ID == ID);

                if (!string.IsNullOrEmpty(transId))
                    query = query.Where(x => x.TransId == transId);

                if(fromDate != null)
                    query = query.Where(x => x.CreatedDate >= fromDate);

                if (toDate != null)
                    query = query.Where(x => x.CreatedDate <= toDate);

                if(status > 0)
                    query = query.Where(x => x.Status == status);
                count = query.Count();
                if(count > 0)
                {
                    int pageNum = (page - 1) * pageSize;
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)count / pageSize));
                    lstData = query.OrderBy(x => x.CreatedDate).Skip(pageNum).Take(pageSize).ToList();
                }    
            }
            return new object[] { lstData, count, totalPage };
        }

        public static tbl_Complain? GetByID(int ID)
        {
            using (var db = new nhanshiphangContext())
            {
                var data = db.tbl_Complains.FirstOrDefault(x => x.ID == ID);
                return data;
            }
        }

        public static DataReturnModel<bool> UpdateStatus(int ID,int status, string userLogin)
        {
            DataReturnModel<bool> result = new();
            try
            {
                using (var db = new nhanshiphangContext())
                {
                    var data = db.tbl_Complains.FirstOrDefault(x => x.ID == ID);
                    if (data == null)
                    {
                        result.IsError = true;
                        result.Data = false;
                        result.Message = "Không tim thấy thông tin về phiếu khiếu nại!";
                        return result;
                    }
                    else
                    {
                        data.Status = status;
                        data.ModifiedBy = userLogin.ToLower();
                        data.ModifiedDate = DateTime.Now;
                        db.Update(data);
                        int kq = db.SaveChanges();
                        if (kq> 0)
                        {
                            result.IsError = false;
                            result.Data = true;
                            result.Message = status == 2 ? "Cập nhật thành công!" : "Từ chối thành công!";
                            var user = AccountBusiness.GetInfo(-1, data.CreatedBy);
                            var admin = AccountBusiness.GetInfo(-1, userLogin);
                            if (user != null && admin != null)
                            {
                                string message = "";
                                string url = $"/Complain/Index?ID={data.ID}";
                                if (status == 2)
                                    message = "Thông tin khiếu nại của bạn đã được hỗ trợ.";
                                else if (status == 3)
                                    message = "Thông tin khiếu nại của bạn đã bị từ chối.";
                                NotificationBusiness.Insert(admin.ID, "Admin", user.ID, user.FullName, -1, data.TransId, message, message, 5, url, admin.Username);
                            }
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                            result.Data = false;
                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi cập nhật phiếu khiếu nại",ex.Message);
                result.IsError = true;
                result.Data = false;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                return result;
            }
        }
    }
}
