using KGQT.Business.Base;
using KGQT.Business;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class FeeWeightController : Controller
    {
        #region Index
        public IActionResult Index()
        {
            var lst = FeeWeight.GetList();
            return View(lst);
        }
        #endregion


        #region Create
        [HttpGet]
        public IActionResult Create() {

            GetDropdownFeeWeightType();
            return View();
        }

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
            GetDropdownFeeWeightType();
            return Json(isSave);
        }
        #endregion

        private List<FeeWeightType> GetDropdownFeeWeightType(int? selectedValue = null)
        {
            var lst = new List<FeeWeightType>();
            var item1 = new FeeWeightType()
            {
                ID = 1,
                Name = "Tuyến bay nhanh"
            };
            var item2 = new FeeWeightType()
            {
                ID = 2,
                Name = "Tuyến bay thường"
            };
            var item3 = new FeeWeightType()
            {
                ID = 3,
                Name = "Tuyến bộ"
            };
            var item4 = new FeeWeightType()
            {
                ID = 4,
                Name = "Tuyến lô"
            };
            var item5 = new FeeWeightType()
            {
                ID = 5,
                Name = "Tuyến biển"
            };
            lst.Add(item1);
            lst.Add(item2);
            lst.Add(item3);
            lst.Add(item4);
            lst.Add(item5);
            ViewBag.Type = new SelectList(lst, "ID", "Name", selectedValue);
            return lst;
        }
    }
    public class FeeWeightType
    {
        public int ID { get; set; }
        public string Name { get; set; }

    }
}
