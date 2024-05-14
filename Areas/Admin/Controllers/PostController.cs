using KGQT.Models.temp;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
using KGQT.Business;
using DocumentFormat.OpenXml.Drawing.Charts;
using KGQT.Commons;
using Newtonsoft.Json;

namespace KGQT.Areas.Admin.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        #region Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        #endregion


        [HttpPost]
        public DataReturnModel<bool> Create(tbl_Posts data)
        {
            if (data is null)
                return new DataReturnModel<bool>() { IsError = true, Data = false, Message = "Tạo bài viết không thành công!" };
            else
            {
                var cookieService = new CookieService(HttpContext);
                var tkck = cookieService.Get("tkck");
                var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
                string userLogin = userModel != null ? userModel.UserName : "";
                data.CraetedBy = userLogin;
                data.CraetedOn = DateTime.Now;
                return PostBusiness.Insert(data);
            }    
        }
    }
}
