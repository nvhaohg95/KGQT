using KGQT.Business;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class FeeWeightController : Controller
    {
        public IActionResult Index()
        {
            ViewData["lstData"] = GetListFeeWeigthCategory();
            return View();
        }

        public IActionResult Detail(int type)
        {
            var lst = FeeWeightBusiness.GetList(type);
            ViewData["type"] = type;
            return View(lst);
        }


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
