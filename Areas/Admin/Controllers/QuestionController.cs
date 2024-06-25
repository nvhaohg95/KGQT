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
    public class QuestionController : Controller
    {
        public IActionResult Index(int page = 1)
        {
            var oData = QuestionBusiness.GetList(1,page,10);
            var lstData = oData[0] as List<tbl_Questions>;
            int numberRecord = (int)oData[1];
            int numberPage = (int)oData[2];
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            return View(lstData);
        }

        public DataReturnModel<bool> Create(string question, string answer, int status)
        {
            DataReturnModel<bool> result = new();
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            if(!string.IsNullOrEmpty(userLogin))
            {
                result = QuestionBusiness.Insert(question, answer, status, userLogin);
            }
            else
            {
                result.IsError = true;
                result.Message = "Thêm câu hỏi không thành công! Vui lòng thử lại";
            }
            return result;
        }

        public DataReturnModel<bool> Delete(int id) {
            DataReturnModel<bool> result = new();
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            if (!string.IsNullOrEmpty(userLogin))
            {
                result = QuestionBusiness.Delete(id, userLogin);
            }
            else
            {
                result.IsError = true;
                result.Message = "Xóa không thành công. Vui lòng thử lại!";
            }
            return result;
        }

        public DataReturnModel<bool> Update(tbl_Questions? data)
        {
            DataReturnModel<bool> result = new();
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            if (data != null)
            {
                result = QuestionBusiness.Update(data, userLogin);
            }
            return result;
        }

        public IActionResult Detail(int id)
        {
            var result = QuestionBusiness.GetByID(id);
            return View(result);
        }

    }
}
