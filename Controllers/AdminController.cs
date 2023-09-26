using KGQT.Business;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace KGQT.Controllers
{
    public class AdminController : Controller
    {
        #region constructor
        private IToastNotification _toastNotification;
        public IToastNotification NotiService
        {
            get { return _toastNotification; }
            set { _toastNotification = value; }
        }

        public AdminController(IToastNotification toastNotification)
        {
            NotiService = toastNotification;
        }
        
        #endregion
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
