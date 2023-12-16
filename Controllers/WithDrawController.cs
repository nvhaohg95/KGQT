using KGQT.Business;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class WithDrawController : Controller
    {
        public bool Create(tbl_Withdraw model)
        {
            string userLogin = HttpContext.Session.GetString("user");
            var user = AccountBusiness.GetInfo(-1, userLogin);
            if (user != null && model != null)
            {
                model.UID = user.ID;
                model.Fullname = user.FullName;
                model.Status = 1;
                model.Type = 1;
                model.CreatedBy = userLogin;
                model.CreatedDate = DateTime.Now;
                return WithDrawBusiness.Insert(model, userLogin);
            }
            return true;
        }
    }
}
