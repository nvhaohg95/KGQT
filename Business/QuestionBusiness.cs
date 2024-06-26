using KGQT.Models;
using KGQT.Models.temp;
using System.Collections.Generic;
using Serilog;
using System.Text.RegularExpressions;
using ILogger = Serilog.ILogger;
namespace KGQT.Business
{
    public static class QuestionBusiness
    {
        private static readonly ILogger _log = Log.ForContext(typeof(AccountBusiness));
        public static object[] GetList(int status, int page, int pageSize)
        {
            using (var db = new nhanshiphangContext())
            {
                List<tbl_Questions> lstData = new();
                int total = 0;
                int totalPage = 0;
                var query = db.tbl_Questions.Where(x => x.Status == status);
                total = query.Count();
                if(total > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                    int pageNum = (page - 1) * pageSize;
                    lstData = query.Skip(pageNum).Take(pageSize).ToList();
                }
                return new object[] { lstData, total, totalPage };
            }
        }

        public static DataReturnModel<bool> Insert(string question, string anwser, int status, string createdBy)
        {
            DataReturnModel<bool> result = new();
            try
            {
                using (var db = new nhanshiphangContext())
                {
                    if (string.IsNullOrEmpty(question))
                    {
                        result.IsError = true;
                        result.Message = "Câu hỏi không được bỏ trống!";
                        return result;
                    }
                    if (string.IsNullOrEmpty(anwser))
                    {
                        result.IsError = true;
                        result.Message = "Câu trả lời không được bỏ trống!";
                        return result;
                    }
                    var admin = db.tbl_Accounts.FirstOrDefault(x => x.Username == createdBy.ToLower());
                    if (admin != null)
                    {
                        tbl_Questions data = new tbl_Questions()
                        {
                            Question = question,
                            Answer = anwser,
                            Status = status,
                            CreatedBy = admin.Username,
                            CreatedOn = DateTime.Now,
                        };
                        db.Add(data);
                        int kq = db.SaveChanges();
                        if(kq > 0)
                        {
                            result.IsError = false;
                            result.Data = true;
                            result.Message = "Thêm câu hỏi thành công!";
                            return result;
                        }
                    }
                    result.IsError = true;
                    result.Message = "Thêm không thành công. Vui lòng thử lại!";
                    return result;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi tạo câu hỏi: ", ex.Message);
                result.IsError = true;
                result.Message = "Thêm không thành công. Vui lòng thử lại!";
                return result;
            }
        }

        public static DataReturnModel<bool> Update(tbl_Questions? dataUpdate, string createdBy)
        {
            DataReturnModel<bool> result = new();
            try
            {
                using (var db = new nhanshiphangContext())
                {
                    if(dataUpdate == null)
                    {
                        result.IsError = true;
                        result.Message = "Câu hỏi không được bỏ trống!";
                        return result;
                    }
                    if (string.IsNullOrEmpty(dataUpdate.Question))
                    {
                        result.IsError = true;
                        result.Message = "Câu hỏi không được bỏ trống!";
                        return result;
                    }
                    if (string.IsNullOrEmpty(dataUpdate.Answer))
                    {
                        result.IsError = true;
                        result.Message = "Câu trả lời không được bỏ trống!";
                        return result;
                    }
                    var admin = db.tbl_Accounts.FirstOrDefault(x => x.Username == createdBy.ToLower());
                    if (admin != null)
                    {
                        var data = db.tbl_Questions.FirstOrDefault(x => x.ID == dataUpdate.ID);
                        data.Question = dataUpdate.Question;
                        data.Answer = dataUpdate.Answer;
                        data.Status = dataUpdate.Status;
                        data.ModifiedBy = admin.Username;
                        data.MidifiedOn = DateTime.Now;
                        db.Update(data);
                        int kq = db.SaveChanges();
                        if (kq > 0)
                        {
                            result.IsError = false;
                            result.Data = true;
                            result.Message = "Cập nhật thành công!";
                            return result;
                        }
                    }
                    result.IsError = true;
                    result.Message = "Cập nhật không thành công. Vui lòng thử lại!";
                    return result;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi cập nhật câu hỏi: ", ex.Message);
                result.IsError = true;
                result.Message = "Cập nhật không thành công. Vui lòng thử lại!";
                return result;
            }
        }
        public static DataReturnModel<bool> Delete(int id, string createdBy)
        {
            DataReturnModel<bool> result = new();
            try
            {
                using (var db = new nhanshiphangContext())
                {
                    var data = db.tbl_Questions.FirstOrDefault(x => x.ID == id);
                    if (data == null)
                    {
                        result.IsError = true;
                        result.Message = "Không tìm thấy câu hỏi. Vui lòng thử lại!";
                    }
                    var admin = db.tbl_Accounts.FirstOrDefault(x => x.Username == createdBy.ToLower());
                    if (admin != null)
                    {
                        data.Status = 2;
                        data.ModifiedBy = admin.Username;
                        data.MidifiedOn = DateTime.Now;
                        db.Update(data);
                        int kq = db.SaveChanges();
                        if (kq > 0)
                        {
                            result.IsError = false;
                            result.Data = true;
                            result.Message = "Xóa câu hỏi thành công!";
                            return result;
                        }
                    }
                    result.IsError = true;
                    result.Message = "Xóa không thành công. Vui lòng thử lại!";
                    return result;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi xóa câu hỏi: ", ex.Message);
                result.IsError = true;
                result.Message = "Xóa không thành công. Vui lòng thử lại!";
                return result;
            }
        }

        public static tbl_Questions? GetByID(int id)
        {
            using (var db = new nhanshiphangContext())
            {
                return db.tbl_Questions.FirstOrDefault(x => x.ID == id);
            }
        }
    }
}
