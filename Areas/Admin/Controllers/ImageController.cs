using DocumentFormat.OpenXml.Wordprocessing;
using KGQT.Business;
using KGQT.Models;
using KGQT.Models.temp;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class ImageController : Controller
    {
        [HttpGet]
        public IActionResult Index(int page)
        {
            var oData = ImageBusiness.GetPage("", 1, 10);
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
        public DataReturnModel<bool> Uploads(List<IFormFile> images,string imageType)
        {
            DataReturnModel<bool> result = new();
            if(images != null && images.Count > 0 && string.IsNullOrEmpty(imageType))
            {
                result = ImageBusiness.UploadImages(images, imageType);
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
}
