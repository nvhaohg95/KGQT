using KGQT.Models.temp;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
using KGQT.Business;
using DocumentFormat.OpenXml.Drawing.Charts;

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
                data.CraetedBy = HttpContext.Session.GetString("user");
                data.CraetedOn = DateTime.Now;
                return PostBusiness.Insert(data);
            }    
        }
    }
}
