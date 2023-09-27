using KGQT.Business;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace KGQT.Controllers
{
    public class UserController : Controller
    {
        #region constructor
        private IToastNotification _toastNotification;
        public IToastNotification NotiService { 
            get { return _toastNotification; }
            set { _toastNotification = value; } 
        }

        public UserController(IToastNotification toastNotification)
        {
            NotiService = toastNotification;
        }
        #endregion

        public ActionResult Index()
        {
            return View();  
        }

        #region LIST ORDER

        #endregion
    }
}
