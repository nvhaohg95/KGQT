using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models;
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

        public ActionResult Details(int id)
        {
            var acc = Accounts.GetInfo(id,"");
            return View(acc);  
        }



        #region LIST ORDER

        #endregion
    }
}
