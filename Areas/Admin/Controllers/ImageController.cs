using DocumentFormat.OpenXml.Wordprocessing;
using KGQT.Business;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing.Printing;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class ImageController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Detail(int imageType, int page = 1, int pageSize = 8)
        {
            var oData = ImageBusiness.GetPage(imageType, page, pageSize);
            var lstData = oData[0] as List<tbl_Images>;
            var numberRecord = (int)oData[1];
            var numberPage = (int)oData[2];
            ViewBag.imageType = imageType;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            return View(lstData);
        }

        [HttpPost]
        public DataReturnModel<tbl_Images> GetByID(string id)
        {
            DataReturnModel<tbl_Images> result = ImageBusiness.GetByID(id);
            return result;
        }

        public DataReturnModel<bool> Uploads(FileUploadModel model)
        {
            DataReturnModel<bool> result = new();
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            if (model != null)
            {
                result = ImageBusiness.UploadImages(model, userLogin);
            }
            else
            {
                result.IsError = true;
                result.Data = false;
                result.Message = "Thêm File không thành công. Vui lòng thử lại!";
            }
            return result;
        }

        [HttpPost]
        public DataReturnModel<List<tbl_Images>> GetImageByType(int imageType)
        {
            DataReturnModel<List<tbl_Images>> result = new();
            result.Data = ImageBusiness.GetImageByType(imageType);
            return result;
        }

        [HttpPost]
        public DataReturnModel<bool> Update(tbl_Images model)
        {
            DataReturnModel<bool> result = new DataReturnModel<bool>();
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            if (!string.IsNullOrEmpty(userLogin))
            {
                result = ImageBusiness.UpdateImages(model, userLogin);
            }
            else
            {
                result.IsError = true;
                result.Data = false;
                result.Message = "Cập nhật File không thành công. Vui lòng thử lại!";
            }
            return result;
        }

        [HttpPost]

        public DataReturnModel<bool> Delete(string id) { 
        
             DataReturnModel<bool> result = new DataReturnModel<bool>();
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            if (!string.IsNullOrEmpty(userLogin))
            {
                result = ImageBusiness.DeleteImage(id);
            }
            else
            {
                result.IsError = true;
                result.Data = false;
                result.Message = "Xóa File không thành công. Vui lòng thử lại!";
            }
            return result; ;
        }
    }
}
