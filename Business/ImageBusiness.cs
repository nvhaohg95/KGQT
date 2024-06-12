using KGQT.Models;
using KGQT.Models.temp;
using Serilog;
using System.Text.RegularExpressions;
using ILogger = Serilog.ILogger;
namespace KGQT.Business
{
    public static class ImageBusiness
    {
        private static readonly ILogger _log = Log.ForContext(typeof(AccountBusiness));

        public static object[] GetPage(string imageType ,int pageIndex,int pageSize)
        {
            using (var db = new nhanshiphangContext())
            {
                List<tbl_Images> datas = new();
                int total = 0;
                int totalPage = 0;
                var query = db.tbl_Images.AsQueryable();
                if (!string.IsNullOrEmpty(imageType))
                    query = query.Where(x => x.ImageType == imageType);
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

        public static DataReturnModel<bool> UploadImages (List<IFormFile> images, string imageType)
        {
            DataReturnModel<bool> result = new();
            try
            {
                if(images.Count > 0)
                {
                    foreach (var image in images)
                    {

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi upload images,", ex.Message);
                result.IsError = true;
                result.Data = false;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại";
                return result;
            }
        }
    }

}
