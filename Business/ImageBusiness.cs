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

        public static DataReturnModel<bool> UploadImages (List<IFormFile> images, int imageType,string createdBy)
        {
            DataReturnModel<bool> result = new();
            try
            {
                if(images == null)
                {
                    result.IsError = true;
                    result.Message = "Vui lòng chọn loại file";
                    return result;
                }
                if(images.Count > 0)
                {
                    string sPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "uploads", "images");
                    if (!Directory.Exists(sPath))
                        Directory.CreateDirectory(sPath);
                    using (var db = new nhanshiphangContext())
                    {
                        foreach (var image in images)
                        {
                            tbl_Images data = new()
                            {
                                RecID = Guid.NewGuid().ToString(),
                                ImageType = imageType,
                                ImageSrc = "",
                                CreatedBy = createdBy,
                                CreatedOn = DateTime.Now
                            };
                            using (var stream = new MemoryStream())
                            {
                                image.CopyTo(stream);
                                var bytes = stream.ToArray();
                                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                                string path = Path.Combine(sPath, fileName);
                                File.WriteAllBytes(path, bytes);
                                data.ImageSrc = "\\" + Path.Combine("uploads", "images", fileName);
                            }
                            db.Add(data);
                        }
                        db.SaveChanges();
                        result.IsError = false;
                        result.Data = true;
                        result.Message = "Thêm thành công!";
                    }
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Không tìm thấy file";
                }
                return result;
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi upload images :" +  ex.Message);
                result.IsError = true;
                result.Data = false;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại";
                return result;
            }
        }

        public static List<tbl_Images> GetImgaeByType(int imageType)
        {
            using (var db = new nhanshiphangContext())
            {
                var datas = db.tbl_Images.Where(x => x.ImageType == imageType && x.Status == 1).ToList();
                return datas;
            }
        }

        public static DataReturnModel<bool> UpdateImages(string recID, int stauts, int viewIndex, string createdBy)
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
                        result.Message = "Không tìm thấy thông tin file";
                        return result;
                    }
                    data.Status = stauts;
                    data.ViewIndex = viewIndex;
                    data.ModifiedBy = createdBy;
                    data.ModifiedOn = DateTime.Now;
                    db.Update(data);
                    int kq = db.SaveChanges();
                    result.IsError = false;
                    result.Data = true;
                    result.Message = "Cập nhật thành công!";
                    return result;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi update images :" + ex.Message);
                result.IsError = true;
                result.Data = false;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại";
                return result;
            }
        }
    }

}
