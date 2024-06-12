using DocumentFormat.OpenXml.Wordprocessing;
using KGQT.Business;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class ImageController : Controller
    {
        [HttpGet]
        public IActionResult Index(int page)
        {
            var oData = ImageBusiness.GetPage(page, 10);
            var lstData = oData[0] as List<tbl_Images>;
            var numberRecord = (int)oData[1];
            var numberPage = (int)oData[2];
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            return View(lstData);
        }

        public DataReturnModel<tbl_Images> GetByID(string id)
        {
            DataReturnModel<tbl_Images> result = ImageBusiness.GetByID(id);
            return result;
        }

        [HttpPost]
        public DataReturnModel<bool> Uploads(int imageType)
        {
            DataReturnModel<bool> result = new();
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var images = Request.Form.Files;
            if (images != null && images.Count > 0)
            {
                result = ImageBusiness.UploadImages(images.ToList(), imageType, userLogin);
            }
            else
            {
                result.IsError = true;
                result.Data = false;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
            }
            return result;
        }


        public DataReturnModel<List<tbl_Images>> GetImageByType(int imageType)
        {
            DataReturnModel<List<tbl_Images>> result = new();
            result.Data = ImageBusiness.GetImgaeByType(imageType);
            return result;
        }
    }
}
