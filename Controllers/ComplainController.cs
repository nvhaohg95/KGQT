using KGQT.Business;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class ComplainController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public DataReturnModel<bool> Create(tbl_Complain data)
        {
            string userName = HttpContext.Request.Cookies["user"];
            var userLogin = AccountBusiness.GetInfo(-1, userName);
            DataReturnModel<bool> result = new();
            if (userLogin != null)
            {
                data.CreatedBy = userLogin.Username;
                data.CreatedDate = DateTime.Now;
                data.Status = 1;
                result = ComplainBusiness.Insert(data);
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
