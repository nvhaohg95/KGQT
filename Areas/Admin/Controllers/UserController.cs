using KGQT.Business;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class UserController : Controller
    {

        public IActionResult Index()
        {
            var lst = UserBusiness.GetListUser();
            return View(lst);
        }

        #region Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        #endregion
    }
}
