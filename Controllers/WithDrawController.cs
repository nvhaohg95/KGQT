using KGQT.Business;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class WithDrawController : Controller
    {
        // gửi yêu cầu nạp tiền
        public DataReturnModel<bool> Create(tbl_Withdraw model)
        {
            string userLogin = HttpContext.Session.GetString("user");
            var user = AccountBusiness.GetInfo(-1, userLogin);
            DataReturnModel<bool> result = new DataReturnModel<bool>();

            if (user != null && model != null)
            {
                model.UID = user.ID;
                model.Fullname = user.FullName;
                model.Status = 1;
                model.Type = 1; // nạp tiền
                model.CreatedBy = userLogin;
                model.CreatedDate = DateTime.Now;
                return WithDrawBusiness.Insert(model, userLogin);
            }
            else
            {
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                result.Data = false;
                return result;
            }
        }
        // gửi yêu cầu rút tiền
        public DataReturnModel<bool> Create2(tbl_Withdraw model)
        {
            string userLogin = HttpContext.Session.GetString("user");
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
    }
}
