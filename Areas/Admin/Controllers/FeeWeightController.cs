using KGQT.Business.Base;
using KGQT.Business;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class FeeWeightController : Controller
    {
        #region Index
        [HttpGet]
        public IActionResult Index(int page = 1,int pageSize = 10)
        {
            var oData = FeeWeightBusiness.GetPage(page, pageSize);
            var lstData = oData[0] as List<tbl_FeeWeight>;
            int totalRecord = (int)oData[1];
            int totalPage = (int)oData[2];
            ViewData["page"] = page;
            ViewData["totalRecord"] = totalRecord;
            ViewData["totalPage"] = totalPage;
            ViewData["lstFeeWeightType"] = GetListFeeWeigthCategory();
            return View(lstData);
        }
        #endregion


        #region Create
        [HttpPost]
        public JsonResult Create(tbl_FeeWeight form)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var user = Accounts.GetInfo(-1, userLogin);
            form.CreatedDate = DateTime.Now;
            form.CreatedBy = user.Username;
            switch (form.Type)
            {
                case 1: // bay nhanh
                case 2: // bay thường
                case 3: // bộ
                    form.MinWeight = 0.3;
                    break;
                case 4: // tuyến lộ
                case 5: // biển
                    form.MinWeight = 40;
                    break;
            }

            var isSave = BusinessBase.Add(form);
            if (isSave)
            {
                BusinessBase.TrackLog(user.ID, form.ID, "{0} đã tạo bảng giá", 0, user.Username);
            }
            return Json(isSave);
        }
        #endregion


        #region Delete
        [HttpPost]
        public bool Delete(int id) 
        {
            var result = FeeWeightBusiness.Delete(id);
            return result;
        }
        #endregion
        private List<FeeweightCategory> GetListFeeWeigthCategory()
        {
            var lst = new List<FeeweightCategory>();
            var item1 = new FeeweightCategory()
            {
                ID = 1,
                Name = "Tuyến bay nhanh"
            };
            var item2 = new FeeweightCategory()
            {
                ID = 2,
                Name = "Tuyến bay thường"
            };
            var item3 = new FeeweightCategory()
            {
                ID = 3,
                Name = "Tuyến bộ"
            };
            var item4 = new FeeweightCategory()
            {
                ID = 4,
                Name = "Tuyến lô"
            };
            var item5 = new FeeweightCategory()
            {
                ID = 5,
                Name = "Tuyến biển"
            };
            lst.Add(item1);
            lst.Add(item2);
            lst.Add(item3);
            lst.Add(item4);
            lst.Add(item5);
            return lst;
        }
    }
    
}
