using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class ServiceController : Controller
    {
        //Bảng giá
        public IActionResult Index()
        {
            var lst = FeeWeight.GetList();
            return View(lst);
        }

        #region Chi tiết bảng giá
        public IActionResult PriceDetail(int id)
        {
            return View();
        }
        #endregion
    }
}
