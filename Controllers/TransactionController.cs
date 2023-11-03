using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class TransactionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        #region Nạp tiền
        public IActionResult Recharge()
        {
            return View();
        }
        #endregion
    }
}
