using KGQT.Business.Base;
using KGQT.Business;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using KGQT.Commons;
using Newtonsoft.Json;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class FeeWeightController : Controller
    {
        #region Index
        [HttpGet]
        public IActionResult Index(int type,int page = 1,int pageSize = 10)
        {
            var oData = FeeWeightBusiness.GetPage(type,page, pageSize);
            var lstData = oData[0] as List<tbl_FeeWeight>;
            int numberRecord = (int)oData[1];
            int numberPage = (int)oData[2];

            ViewBag.type = type;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            ViewBag.lstFeeWeightType = GetListFeeWeigthCategory();

            return View(lstData);
        }
        #endregion

        #region Create
        [HttpPost]
        public JsonResult Create(tbl_FeeWeight form)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var user = AccountBusiness.GetInfo(-1, userLogin);
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
                BusinessBase.SysLog(user.ID, 1, "{0} đã tạo bảng giá", "", user.Username);
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

        #region Get By ID
        public tbl_FeeWeight? GetByID(int id)
        {
            if(id < 0) return null;
            var data =  FeeWeightBusiness.GetByID(id);
            return data;
        }
        #endregion

        #region Update
        public bool Update(tbl_FeeWeight model)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            if (model != null && !string.IsNullOrEmpty(userLogin))
            {
                var result = FeeWeightBusiness.Update(model, userLogin);
                return result;
            }
            return false;
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
            
            lst.Add(item1);
            lst.Add(item2);
            lst.Add(item3);
            lst.Add(item4);
            return lst;
        }
    }
    
}
