using Fasterflect;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Newtonsoft.Json;
using Serilog;
using System.Text.RegularExpressions;
using ILogger = Serilog.ILogger;
namespace KGQT.Business
{
    public static class ImageBusiness
    {
        private static readonly ILogger _log = Log.ForContext(typeof(AccountBusiness));

        public static object[] GetPage(int imageType,int pageIndex,int pageSize)
        {
            using (var db = new nhanshiphangContext())
            {
                List<tbl_Images> datas = new();
                int total = 0;
                int totalPage = 0;
                var query = db.tbl_Images.Where(x => x.ImageType == imageType);
                total = query.Count();
                if(total > 0)
                {
                    query = query.OrderByDescending(x => x.CreatedOn);
                    int pageNum = (pageIndex - 1) * pageSize;
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                    datas = query.Skip(pageNum).Take(pageSize).ToList();   
                }
                return new object[] {datas, total,totalPage};
            };
        }
        public static DataReturnModel<tbl_Images> GetByID(string id)
        {
            DataReturnModel<tbl_Images> result = new();
            using (var db = new nhanshiphangContext())
            {
                var data = db.tbl_Images.FirstOrDefault(x => x.RecID == id);
                if(data != null)
                {
                    result.IsError = false;
                    result.Data = data;
                }
                else
                {
                    result.IsError = true;
                    result.Data = null;
                    result.Message = "Không tìm thấy thông tin hình ảnh.";
                }
                return result;
            };
        }
        public static List<tbl_Images> GetImageByType(int imageType)
        {
            using (var db = new nhanshiphangContext())
            {
                var datas = db.tbl_Images.Where(x => x.ImageType == imageType && x.Status == 1).ToList();
                return datas;
            }
        }
        public static DataReturnModel<bool> UploadImages (FileUploadModel fileUpload,string createdBy)
        {
            DataReturnModel<bool> result = new();
            try
            {
                if(fileUpload == null)
                {
                    result.IsError = true;
                    result.Message = "File tải lên không hợp lệ!";
                    return result;
                }
                string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "uploads", "images");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                using (var db = new nhanshiphangContext())
                {
                    tbl_Images data = new()
                    {
                        RecID = Guid.NewGuid().ToString(),
                        ImageType = fileUpload.FileType,
                        ImageSrc = "",
                        CreatedBy = createdBy,
                        CreatedOn = DateTime.Now
                    };
                    using (var stream = new MemoryStream())
                    {
                        fileUpload.File.CopyTo(stream);
                        var bytes = stream.ToArray();
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileUpload.FileName);
                        string path = Path.Combine(folder, fileName);
                        File.WriteAllBytes(path, bytes);
                        data.ImageSrc = "\\" + Path.Combine("uploads", "images", fileName);
                    }
                    db.Add(data);
                    int kq = db.SaveChanges();
                    if (kq > 0)
                    {
                        result.IsError = false;
                        result.Data = true;
                        result.Message = "Thêm File thành công!";
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Thêm File không thành công!";
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi upload file :" +  ex.Message);
                result.IsError = true;
                result.Data = false;
                result.Message = "Thêm File không thành công. Vui lòng thử lại";
                return result;
            }
        }

        public static DataReturnModel<bool> UpdateImages(tbl_Images model, string createdBy)
        {
            DataReturnModel<bool> result = new();
            try
            {
                using (var db = new nhanshiphangContext())
                {
                    var data = db.tbl_Images.FirstOrDefault(x => x.RecID == model.RecID);
                    if (data == null)
                    {
                        result.IsError = true;
                        result.Message = "Không tìm thấy File!";
                        return result;
                    }
                    data.Status = model.Status;
                    data.ViewIndex = model.ViewIndex;
                    data.ModifiedBy = createdBy;
                    data.ModifiedOn = DateTime.Now;
                    db.Update(data);
                    int kq = db.SaveChanges();
                    if(kq > 0)
                    {
                        result.IsError = false;
                        result.Data = true;
                        result.Message = "Cập nhật File thành công!";
                    }
                    else
                    {
                        result.IsError = true;
                        result.Data = false;
                        result.Message = "Cập nhật File không thành công. Vui lòng thử lại!";
                    }    
                    return result;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi update images :" + ex.Message);
                result.IsError = true;
                result.Data = false;
                result.Message = "Cập nhật File không thành công. Vui lòng thử lại";
                return result;
            }
        }

        public static DataReturnModel<bool> DeleteImage(string recID)
        {
            DataReturnModel<bool> result = new();
            try
            {
                using (var db = new nhanshiphangContext())
                {
                    var data = db.tbl_Images.FirstOrDefault(x => x.RecID == recID);
                    if (data == null)
                    {
                        result.IsError = true;
                        result.Message = "Không tìm thấy File";
                        return result;
                    }
                    string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
                    string path = string.Concat(folder,data.ImageSrc);
                    if (File.Exists(path))
                        File.Delete(path);
                    db.Remove(data);
                    int kq = db.SaveChanges();
                    if(kq > 0)
                    {
                        
                        
                        result.IsError = false;
                        result.Data = true;
                        result.Message = "Xóa File thành công!";
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Xóa File không thành công. Vui lòng thử lại!";
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi xóa images :" + ex.Message);
                result.IsError = true;
                result.Data = false;
                result.Message = "Xóa File không thành công. Vui lòng thử lại";
                return result;
            }
        }
    }

}
