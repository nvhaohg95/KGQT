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

            GetListFeeWeightType();
            return View();
        }
        [HttpPost]
        public JsonResult Create(tbl_FeeWeight form)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var user = Accounts.GetInfo(-1, userLogin);
            form.MinWeight = form.WeightFrom;
            form.CreatedDate = DateTime.Now;
            form.CreatedBy = user.Username;
            var isSave = BusinessBase.Add(form);
            if (isSave)
            {
                BusinessBase.TrackLog(user.ID, form.ID, "{0} đã tạo bảng giá", 0, user.Username);
            }
            GetListFeeWeightType();
            return Json(isSave);
        }
        #endregion

        private List<FeeWeightType> GetListFeeWeightType(int? selectedValue = null)
        {
            var lst = new List<FeeWeightType>();
            var item1 = new FeeWeightType()
            {
                ID = 1,
                Name = "Gói bay nhanh"
            };
            var item2 = new FeeWeightType()
            {
                ID = 2,
                Name = "Gói bay thường"
            };
            var item3 = new FeeWeightType()
            {
                ID = 3,
                Name = "Gói bộ"
            };
            lst.Add(item1);
            lst.Add(item2);
            lst.Add(item3);
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
