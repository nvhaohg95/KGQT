using KGQT.Business;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Controllers
{
    public class WithDrawController : Controller
    {
        // gửi yêu cầu nạp tiền
        public DataReturnModel<bool> Create(tbl_Withdraw model)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var user = AccountBusiness.GetInfo(-1, userLogin);
            DataReturnModel<bool> result = new DataReturnModel<bool>();
            if (user != null && model != null)
            {
                model.UID = user.ID;
                model.Fullname = user.FullName;
                model.Status = 1;
                model.Type = 1;
                model.CreatedBy = userLogin;
                model.CreatedDate = DateTime.Now;
                return WithDrawBusiness.Insert(model, userLogin);
            }
            else
            {
                result.IsError = true;
                result.Data = false;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                return result;
            }
        }
        
        // gửi yêu cầu rút tiền
        public DataReturnModel<bool> Create2(tbl_Withdraw model)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var user = AccountBusiness.GetInfo(-1, userLogin);
            DataReturnModel<bool> result = new DataReturnModel<bool>();
            if (user != null && model != null)
            {
                model.UID = user.ID;
                model.Fullname = user.FullName;
                model.Status = 1;
                model.Type = 2; 
                model.CreatedBy = userLogin;
                model.CreatedDate = DateTime.Now;
                return WithDrawBusiness.Insert2(model, userLogin);
            }
            else
            {
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                result.Data = false;
                return result;
            }
        }


        public DataReturnModel<bool> BuySearches (int amount)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var user = AccountBusiness.GetInfo(-1, userLogin);
            DataReturnModel<bool> result = new DataReturnModel<bool>();
            if (user != null)
            {
                return WithDrawBusiness.BuySearches(user.Username, amount, "");
            }
            else
            {
                result.IsError = true;
                result.Data = false;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                return result;
            }
        }
    }
}
