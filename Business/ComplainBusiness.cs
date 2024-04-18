
using DocumentFormat.OpenXml.Wordprocessing;
using KGQT.Models;
using KGQT.Models.temp;

namespace KGQT.Business
{
    public static class ComplainBusiness
    {
        public static DataReturnModel<bool> Insert(tbl_Complain data)
        {
            DataReturnModel<bool> result = new();
            if(data == null)
            {
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                result.Data = false;
                return result;
            }
            using (var db = new nhanshiphangContext())
            {
                db.Add(data);
                var isSave = db.SaveChanges();
                if(isSave > 0)
                {
                    result.IsError = false;
                    result.Data = true;
                    result.Message = "Yêu cầu của bạn đã được gửi";
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

        public static object[] GetList(string transId, DateTime? fromDate, DateTime? toDate,int? status, int page, int pageSize = 10)
        {
            List<tbl_Complain> lstData = new();
            int count = 0;
            int totalPage = 0;

            using (var db = new nhanshiphangContext())
            {
                var query = db.tbl_Complains.AsEnumerable();
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

        public static DataReturnModel<bool> UpdateStatus(int ID,int status, string userName)
        {
            DataReturnModel<bool> result = new();
            using(var db = new nhanshiphangContext())
            {
                var data = db.tbl_Complains.FirstOrDefault(x => x.ID == ID);
                if(data == null)
                {
                    result.IsError = true;
                    result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                    result.Data = false;
                }
                else
                {
                    data.Status = status;
                    data.ModifiedBy = userName;
                    data.ModifiedDate = DateTime.Now;
                    db.Update(data);
                    int isSave = db.SaveChanges();
                    if (isSave > 0)
                    {
                        result.IsError = false;
                        result.Data = true;
                        result.Message = status == 2 ? "Cập nhật thành công!" : "Từ chối thành công!";
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                        result.Data = false;
                    }
                }    
                return result;
            }
        }
    }
}
