using KGQT.Business.Base;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class PackageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create(int id)
        {
            var order = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.ID == id);
            return View(order);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var package = BusinessBase.GetOne<tbl_Package>(x => x.ID == id);
            return View(package);
        }

        [HttpGet]
        public List<tbl_Package> ScanPackage(string str)
        {
            return BusinessBase.GetList<tbl_Package>(x => x.PackageCode.Contains(str)).ToList();
        }
    }
}
